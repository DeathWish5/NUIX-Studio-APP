﻿using System.Collections;
using UnityEngine;
using EvtSource;
using Proyecto26;
using System.IO;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using System;

/// <summary>
/// Responsible for updating the state of the item
/// Usually is attached to the ItemWidget
/// </summary>
public class ItemController
{
    public string ItemId { get; set; } // name of item in openhab
    public EvtType SubscriptionType { get; set; } //Subscribe to this eventtype or if none, don't subscribe

    public ItemController(string itemID = "None", EvtType evtType = EvtType.None)
    {
        ItemId = itemID;
        SubscriptionType = evtType;
    }

    public delegate void OnItemUpdate();
    public OnItemUpdate updateItem;

    /// <summary>
    /// Eventcontroller passes an event to ItemController
    /// </summary>
    /// <param name="evt">the event as eventmodel</param>
    public void ReceivedEvent(EventModel evt)
    {
        if (SemanticModel.getInstance().items[ItemId].ItemModel.state != evt._Payload.value) SemanticModel.getInstance().items[ItemId].ItemModel.state = evt._Payload.value;
        Debug.Log("Event value: " + evt._Payload.value + " New Item state value: " + SemanticModel.getInstance().items[ItemId].ItemModel.state);
        updateItem?.Invoke(); // Send event to Widgets for UI updates
    }

    /// <summary>
    /// Make a REST call and propagate this object with data
    /// </summary>
    private void GetItemFromServer()
    {
        if (SemanticModel.getInstance().items.ContainsKey(ItemId))
        {
            SemanticModel.getInstance().items[ItemId].ItemModel = SemanticModel.getInstance().items[ItemId].ItemModel;
        }
        else
        {
            Debug.Log("No item " + ItemId + "in semantic model");

            RestClient.Get(UriBuilder.GetItemUri(ClientConfig.getInstance()._ServerURL, ItemId)).Then(response =>
            {
                if (response.StatusCode >= 200 && response.StatusCode < 300)
                {
                    SemanticModel.getInstance().items[ItemId].ItemModel = JsonUtility.FromJson<EnrichedGroupItemDTO>(response.Text);
                    Debug.Log("Rest GET status for Item: " + SemanticModel.getInstance().items[ItemId].ItemModel.name + " was OK");
                    updateItem?.Invoke(); // Send event to Widgets
                    //SemanticModel.getInstance().AddWidget(itemId, _Item);
                    // ADD the widget to semantic model
                }
                else
                {
                    Debug.Log("Rest GET status for Item: " + ItemId + " was not in OK span. (" + response.StatusCode + ")\n" + response.Error);
                }
            });
        }
    }

    /// <summary>
    /// Update item state with POST method (plaintext)
    /// </summary>
    /// <param name="newState">New state</param>
    private void SetItemOnServer(string newState)
    {
        //OnItemUpdated = false;
        Debug.Log("New state: " + newState);
        RestClient.DefaultRequestHeaders["content-type"] = "text/plain";
        RestClient.Post(UriBuilder.GetItemUri(ClientConfig.getInstance()._ServerURL, ItemId), newState).Then(response => {
            Debug.Log("Posted " + newState + " to " + ItemId + ". With responsecode " + response.StatusCode);
            //OnItemUpdated = true;
            RestClient.ClearDefaultHeaders();
        });
    }


    // TODO: Create a widget for the item on Success calback
    /// <summary>
    /// Creates an item on server with PUT method (GroupItemDTO item)
    /// </summary>
    /// <param name="item"></param>
    public void CreateItemOnServer(GroupItemDTO item)
    {
        RestClient.DefaultRequestHeaders["Authorization"] = ClientConfig.getInstance().Authenticate();
        RestClient.DefaultRequestHeaders["content-type"] = "application/json";
        RequestHelper currentRequest;
        currentRequest = new RequestHelper
        {
            Uri = UriBuilder.GetItemUri(ClientConfig.getInstance()._ServerURL, item.name),
            Body = item,
            Retries = 5,
            RetrySecondsDelay = 1,
            RetryCallback = (err, retries) =>
            {
                Debug.Log(string.Format("Retry #{0} Status {1}\nError: {2}", retries, err.StatusCode, err));
            }
        };

        RestClient.Put<string>(currentRequest, (err, res, body) =>
        {
            if (err != null)
            {
                Debug.Log("Error creating item on server");
                Debug.Log(err.Message);
            }
            else
            {
                Debug.Log("Success " + res.StatusCode + JsonUtility.ToJson(body, true));
            }
            RestClient.ClearDefaultHeaders();
        });
    }


    /// <summary>
    /// Return Item state (value) as string
    /// </summary>
    /// <returns>string with current state</returns>
    public string GetItemStateAsString()
    {
        return SemanticModel.getInstance().items[ItemId].ItemModel.state.ToString();
    }

    /// <summary>
    /// Update text item on server
    /// </summary>
    /// <param name="value"></param>
    public void SetItemStateAsString(string value)
    {
        SetItemOnServer(value);
    }

    /// <summary>
    /// Try to parse the state as a float
    /// </summary>
    /// <returns>result if float parse try</returns>
    public float GetItemStateAsFloat()
    {
        float result;
        float.TryParse(SemanticModel.getInstance().items[ItemId].ItemModel.state, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CurrentCulture, out result);
        return result;
    }

    /// <summary>
    /// Parse item value to integer. Assume dimmer state is between
    /// 0 - 100. If not, return -1 for error. (Disable slider in that case)
    /// </summary>
    /// <returns>Dimmer as percent in integer</returns>
    public float GetItemStateAsDimmer()
    {
        float value;
        if (SemanticModel.getInstance().items[ItemId].ItemModel.state == "NULL") return -1f;
        value = float.Parse(SemanticModel.getInstance().items[ItemId].ItemModel.state);
        //Debug.Log("Value parsed to: " + value);
        if (value >= 0f && value <= 100f)
        {
            return value;
        }
        else
        {
            return -1f;
        }
    }

    /// <summary>
    /// Update dimmer item on server
    /// </summary>
    /// <param name="value">Needs to be integer between or equal to 0 and 100</param>
    public void SetItemStateAsDimmer(int value)
    {
        if (value >= 0 && value <= 100) SetItemOnServer(value.ToString());
    }

    /// <summary>
    /// Try to parse the state as a switch
    /// </summary>
    /// <returns>Switch state as bool (ON = True, OFF=False)</returns>
    public bool GetItemStateAsSwitch()
    {
        if (SemanticModel.getInstance().items[ItemId].ItemModel.state == "ON") return true;
        return false;
    }
    /*
    public Color GetItemStateAsColor()
    {
        float[] HSB = Array.ConvertAll(SemanticModel.getInstance().items[itemId].itemModel.state.Split(','), float.Parse);
        Color colorState = Color.HSVToRGB(HSB[0], HSB[1], HSB[2]);
    }*/

    /// <summary>
    /// Update item on server as a switch
    /// </summary>
    /// <param name="newState">bool True/False On/Off</param>
    public void SetItemStateAsSwitch(bool newState)
    {
        if (newState)
        {
            SetItemOnServer("ON");
        }
        else
        {
            SetItemOnServer("OFF");
        }
    }

    /// <summary>
    /// Set a Player Item Play/Pause state
    /// </summary>
    /// <param name="newState"></param>
    public void SetItemStateAsPlayerPlayPause(bool newState)
    {
        if (newState)
        {
            SetItemOnServer("PLAY");
        }
        else
        {
            SetItemOnServer("PAUSE");
        }
    }

    /// <summary>
    /// Search functions on Player Item
    /// </summary>
    /// <param name="cmd">ff = Fastforward, rw = Rewind, next = Next, prev = Previous</param>
    public void SetItemStateAsPlayerSearch(string cmd)
    {
        switch (cmd)
        {
            case "ff":
                SetItemOnServer("FASTFORWARD");
                break;
            case "rw":
                SetItemOnServer("REWIND");
                break;
            case "next":
                SetItemOnServer("NEXT");
                break;
            case "prev":
                SetItemOnServer("PREVIOUS");
                break;
            default:
                Debug.Log("Command is not recognized by player item");
                break;
        }
    }

    public void SetItemStateAsColor(Color newColor)
    {
        float Hue;
        float Saturation;
        float Brightness;
        Color.RGBToHSV(newColor, out Hue, out Saturation, out Brightness);

        Debug.Log(Hue.ToString() + ", " + Saturation.ToString() + ", " + Brightness.ToString());
        SetItemOnServer((Hue * 360).ToString() + "," + (Saturation * 100).ToString() + "," + (Brightness*100).ToString());
    }

    public Vector3 StringToVec(string s)
    {
        Debug.Log("Converting string to vector: " + s);
        if (s == "NULL") return SemanticModel.getInstance().SpawnPosition;
        string[] temp = s.Substring(1, s.Length - 2).Split(',');
        if (temp.Length == 3) return new Vector3(float.Parse(temp[0]), float.Parse(temp[1]), float.Parse(temp[2]));
        else return SemanticModel.getInstance().SpawnPosition;
    }
    public Vector3 GetItemStateAsVector()
    {
        if (SemanticModel.getInstance().items[ItemId].ItemModel.state != null) return StringToVec(SemanticModel.getInstance().items[ItemId].ItemModel.state);
        else return Vector3.zero;
    }

    public void SetItemStateAsVector(Vector3 value)
    {
        SetItemOnServer(value.ToString());
    }

    /// <summary>
    /// Getter for item ID
    /// </summary>
    /// <returns></returns>
    public string GetItemID()
    {
        return ItemId;
    }

    /// <summary>
    /// Getter for subtype
    /// </summary>
    /// <returns></returns>
    public EvtType GetItemSubType()
    {
        return SubscriptionType;
    }
}
/// <summary>
/// EventTypes available from Event stream of items
/// </summary>
public enum EvtType
{
    ItemStateChangedEvent,
    ItemStateEvent,
    ItemCommandEvent,
    ItemUpdatedEvent,
    ItemRemovedEvent,
    ItemAddedEvent,
    None

    /**
    ItemAddedEvent          /added
    ItemRemovedEvent        /removed
    ItemUpdatedEvent        /updated
    ItemCommandEvent        /command
    ItemStateEvent          /state
    ItemStateChangedEvent   /statechanged   (This is 99% of times what we want)
    **/
}