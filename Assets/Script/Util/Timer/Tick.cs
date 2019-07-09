﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utils
{
    public delegate void TimerCallBack(int Count);

    public class Tick
    {
        public int tid;
        public float start;
        public int count;        
        public float interval;
        public TimerCallBack cbfunc;
        public bool pause;
        public bool fix;
    }
}
