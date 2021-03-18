﻿using Proyecto26;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

class SemanticModelController : MonoBehaviour
{
    [SerializeField] 
    public GameObject _dimmerWidgetPrefab;
    public GameObject _switchWidgetPrefab;
    public GameObject _textWidgetPrefab;
    public GameObject _locationWidgetPrefab;
    public GameObject _equipmentWidgetPrefab;
    public GameObject _colorWidgetPrefab;

    [Header("Category prefabs")]
    public GameObject _lampPrefab;
    public GameObject _mobilePhonePrefab;

    [Header("Tag-based interactables")]
    public GameObject _dimmerGestureControlPrefab;
    public GameObject _dimmerBrightnessWidgetPrefab;
    public GameObject _virtualLocationControlPrefab;

    public bool InitOnStartup = false;



    private void Start()
    {
        if (InitOnStartup)
        {
            StartSystem();
        }
    }
    public void StartSystem()
    {
        print("Starting System");
        InitializeWidgetPrefabDictionary();
        GetModel();
    }

    private void GetModel()
    {
        //GetThingNames();
        GetComponent<EventController>().StartListen();
        GetAllItems();
    }

    private void Update()
    {
        // Otherwise call it after all the widgets are added to the scene
        // Currently the complexity is O(n)
        SetParentTransforms();
    }


    public void GetAllItems()
    {
        RestClient.Get(ClientConfig.getInstance()._ServerURL + "/rest/items").Then(res => {
            if (res.StatusCode >= 200 && res.StatusCode < 300)
            {
                List<EnrichedGroupItemDTO> equipmentItems = JsonUtility.FromJson<EquipmentItemModelList>("{\"result\":" + res.Text + "}").result;


                foreach (EnrichedGroupItemDTO equipmentItemModel in equipmentItems)
                {
                    List<GameObject> Widgets = new List<GameObject>();
                    Widgets.AddRange(CreateWidgetsByPrefab(equipmentItemModel));
                    SemanticModel.getInstance().AddItem(equipmentItemModel, Widgets);
                }
            }
            else
            {
                Debug.Log("Rest GET status for Item: " + " was not in OK span. (" + res.StatusCode + ")\n" + res.Error);
            }
        });
    }

    private void InitializeWidgetPrefabDictionary()
    {
        if (_dimmerWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Dimmer"] = _dimmerWidgetPrefab;
        if (_switchWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Switch"] = _switchWidgetPrefab;
        if (_textWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["String"] = _textWidgetPrefab;
        if (_locationWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["VirtualLocation"] = _locationWidgetPrefab;
        if (_equipmentWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Group"] = _equipmentWidgetPrefab;
        if (_colorWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Color"] = _colorWidgetPrefab;


        // There are plenty of Number:<dimension> units https://www.openhab.org/docs/concepts/units-of-measurement.html
        // Not going to add a dict entry for each of them for now
        if (_textWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Number:Time"] = _textWidgetPrefab;
        if (_textWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Number"] = _textWidgetPrefab;
        if (_textWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["DateTime"] = _textWidgetPrefab;


        // Tag-Based interactables
        if (_dimmerGestureControlPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Dimmer#NUIXGestureControl"] = _dimmerGestureControlPrefab;
        if (_dimmerBrightnessWidgetPrefab != null) ClientConfig.getInstance()._widgetPrefabs["Dimmer#NUIXBrightness"] = _dimmerBrightnessWidgetPrefab;
        if (_virtualLocationControlPrefab != null) ClientConfig.getInstance()._widgetPrefabs["LocationControl"] = _virtualLocationControlPrefab;
    }


    public List<GameObject> CreateWidgetsByPrefab(EnrichedGroupItemDTO item)
    {
        List<GameObject> itemWidgets = new List<GameObject>();

        if (ClientConfig.getInstance()._widgetPrefabs.ContainsKey(item.type))
        {
            GameObject itemWidget;
            itemWidget = Instantiate(ClientConfig.getInstance()._widgetPrefabs[item.type], this.transform.position, Quaternion.identity) as GameObject;
            itemWidget.GetComponent<ItemWidget>().item = item.name;
            itemWidget.name = item.name + " Widget";
            itemWidgets.Add(itemWidget);
        }

        foreach (string itemTag in item.tags)
        {
            if (ClientConfig.getInstance()._widgetPrefabs.ContainsKey(item.type + "#" + itemTag))
            {
                print(item.type + "#" + itemTag);
                GameObject itemWidget;
                itemWidget = Instantiate(ClientConfig.getInstance()._widgetPrefabs[item.type + "#" + itemTag], this.transform.position, Quaternion.identity) as GameObject;
                itemWidget.GetComponent<ItemWidget>().item = item.name;
                itemWidget.name = item.name + "#" + itemTag + " Widget";
                itemWidgets.Add(itemWidget);
            }
        }
        return itemWidgets;
    }


    public void InstantiateWidget(Item item)
    {

        string itemname = item.itemModel.name;
        print("Instantiating widgets for " + itemname);
        if (ClientConfig.getInstance()._widgetPrefabs.ContainsKey(item.itemModel.category))
        {
            GameObject createdItem;
            createdItem = Instantiate(ClientConfig.getInstance()._widgetPrefabs[item.itemModel.category], this.transform.position, Quaternion.identity) as GameObject;
            createdItem.GetComponent<ItemWidget>().item = itemname;
            createdItem.name = itemname + " Widget";
            item.AddWidget(createdItem);
        }
        else if (ClientConfig.getInstance()._widgetPrefabs.ContainsKey(item.itemModel.type))
        {
            GameObject createdItem;
            createdItem = Instantiate(ClientConfig.getInstance()._widgetPrefabs[item.itemModel.type], this.transform.position, Quaternion.identity) as GameObject;
            createdItem.GetComponent<ItemWidget>().item = itemname;
            createdItem.name = itemname + " Widget";
            item.AddWidget(createdItem);
        }
        
        foreach (string itemTag in item.itemModel.tags)
        {
            if (itemTag == "GestureControlled")
            {
                GameObject createdItem;
                createdItem = Instantiate(ClientConfig.getInstance()._widgetPrefabs["GestureControlledDimmer"], Vector3.zero, Quaternion.identity) as GameObject;
                createdItem.GetComponent<ItemWidget>().item = itemname;
                createdItem.name = itemname + " GestureControlledWidget";
                item.AddWidget(createdItem);
            }
            else if (itemTag == "LightDimmer")
            {
                // Need to assign the light component
                GameObject createdItem;
                createdItem = Instantiate(ClientConfig.getInstance()._widgetPrefabs["LightDimmer"], this.transform.position, Quaternion.identity) as GameObject;
                createdItem.GetComponent<ItemWidget>().item = itemname;
                createdItem.name = itemname + " LightDimmerWidget";
                item.AddWidget(createdItem);
            }
            else if (itemTag == "LocationControl")
            {
                // Need to create locationWidgets
                //GameObject createdItem;
                //createdItem = Instantiate(ClientConfig.getInstance()._widgetPrefabs["LocationControl"], this.transform.position + Vector3.down / 5f, Quaternion.identity) as GameObject;
                //createdItem.GetComponent<ItemWidget>()._Item = itemname + "VirtualLocation";
                //createdItem.name = itemname + "VirtualLocation Widget";
                //item.AddWidget(createdItem);


                List<string> asGroup = new List<string>();
                asGroup.Add(itemname);

                GroupItemDTO locationItem = new GroupItemDTO
                {
                    type = "String",
                    name = itemname + "VirtualLocation",
                    label = itemname + "VirtualLocation",
                    category = "VirtualLocation",
                    groupNames = asGroup
                };
                CreateItemOnServer(locationItem);

            }
        }
        


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
            RetryCallback = (err, retries) => {
                Debug.Log(string.Format("Retry #{0} Status {1}\nError: {2}", retries, err.StatusCode, err));
            }
        };

        RestClient.Put<string>(currentRequest, (err, res, body) => {
            if (err != null)
            {
                print("Error creating item on server");
                print(err.Message);
            }
            else
            {
                print("Success " + res.StatusCode + JsonUtility.ToJson(body, true));
            }
            RestClient.ClearDefaultHeaders();
        });

    }

    private void CreateWidgetsForItems()
    {
        foreach (KeyValuePair<string, Item> item in SemanticModel.getInstance().items)
        {
            InstantiateWidget(item.Value);
        }
    }

    private void SetParentTransforms()
    {
        // setting the parent based on groupnames

        foreach (KeyValuePair<string, Item> item in SemanticModel.getInstance().items)
        {
            if (item.Value._itemWidgets != null)
            {
                // The item should be a child of its groupname item. If we find at least one such groupname, set the transform parent to its widget then
                foreach (string groupName in item.Value.itemModel.groupNames)
                {
                    GameObject parent;
                    if ((parent = GameObject.Find(groupName + " Widget")) != null)
                    {
                        foreach (GameObject itemWidget in item.Value._itemWidgets)
                        {
                            itemWidget.transform.SetParent(parent.transform);
                        }
                    }
                }
            }
        }
    }


    public void SetServerIPAdress()
    {
        string adr = GameObject.Find("NativeKeyboardOutputIPAdress").GetComponent<TMPro.TextMeshPro>().text;
        //string adr = GameObject.Find("KeyboardOutputIPAdress").GetComponent<TMP_InputField>().text;
        print(adr);
        if (System.Net.IPAddress.TryParse(adr, out var _))
        {
            print("DEBUG IP ADRESS " + adr);
            ClientConfig.getInstance()._ServerURL = "http://" + adr;
        }
        else
        {
            print("Invalid IP Address");
            ClientConfig.getInstance()._ServerURL = "http://" + adr;
        }
    }

    //Move it away from this class; make loading of prefabs in semanticmodelcontroller using this method by default
    private GameObject LoadPrefabFromFile(string filename)
    {
        Debug.Log("Trying to load LevelPrefab from file (" + filename + ")...");
        var loadedObject = Resources.Load(filename);
        if (loadedObject == null)
        {
            throw new FileNotFoundException("...no file found - please check the configuration");
        }
        return loadedObject as GameObject;
    }


    private void CreateWidget()
    {

    }
}