using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    /// <summary>
    /// Shared between Tower Defense and Dating simulator
    /// An interpreter on either side can make a character out of them.
    /// </summary>
    [System.Serializable]
    public class Characteristic
    {
        public enum RootCharacteristic { Face, Hat};
        public enum TreeCharacteristic { ShootingPower, Height };
        public float value;
    }

    //A shared character is made up of a list of characteristic
    public Characteristic[] character;
}
