using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public class Characteristic
    {
        public enum RootCharacteristic { Face, Hat};
        public enum TreeCharacteristic { ShootingPower, Height };
        public float value;
    }
}
