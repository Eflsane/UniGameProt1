using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UserParams
{
    public string userName;

    public float score = -1;
    public float income;

    public int premMoney = 0;

    public long gameQuitTimeTicks;

    public int speed = 1;
    public double speedBoosterWorkingTimeLeftSeconds;
    public long speedBoosterStartTimeTicks;

    public int gridLevel;

    public UserParams()
    {

    }

    public UserParams(string userName):this()
    {
        this.userName = userName;
    }
}
