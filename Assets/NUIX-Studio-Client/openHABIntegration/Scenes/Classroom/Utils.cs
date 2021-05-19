using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{   
    public class DebugState
    {
        enum TRIGGER
        {
            PLAYPAUSE = 0, FASTFORWARD = 1,
        }
        const int TRIGGER_MAX = 1;

        bool[] trigger;

        DebugState()
        {
            trigger = new bool[TRIGGER_MAX + 1];
        }

        void setTrigger(int n, bool state)
        {
            if (n > TRIGGER_MAX || n < 0) {
                return;
            }
            trigger[n] = state;
        }
    }
}

