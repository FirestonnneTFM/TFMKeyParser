using System;

namespace TFMStone.ABC
{
    internal enum ABCOp : byte
    {
        bkpt = 0x01,
        nop = 0x02,
        thrw = 0x03,
        getsuper = 0x04,
        setsuper = 0x05,
        dxns = 0x06,
        dxnslate = 0x07,
        kill = 0x08,
        label = 0x09,
        ifnlt = 0x0c,
        ifnle = 0x0d,
        ifngt = 0x0e,
        ifnge = 0x0f,
        jump = 0x10,
        iftrue = 0x11,
        iffalse = 0x12,
        ifeq = 0x13,
        ifne = 0x14,
        iflt = 0x15,
        ifle = 0x16,
        ifgt = 0x17,
        ifge = 0x18,
        ifstricteq = 0x19,
        ifstrictne = 0x1a,
        lookupswitch = 0x1b,
        pushwith = 0x1c,
        popscope = 0x1d,
        nextname = 0x1e,
        hasnext = 0x1f,
        pushnull = 0x20,
        pushundefined = 0x21,
        nextvalue = 0x23,
        pushbyte = 0x24,
        pushshort = 0x25,
        pushtrue = 0x26,
        pushfalse = 0x27,
        pushnan = 0x28,
        pop = 0x29,
        dup = 0x2a,
        swap = 0x2b,
        pushstring = 0x2c,
        pushint = 0x2d,
        pushuint = 0x2e,
        pushdouble = 0x2f,
        pushscope = 0x30,
        pushnamespace = 0x31,
        hasnext2 = 0x32,
        li8 = 0x35,
        li16 = 0x36,
        li32 = 0x37,
        lf32 = 0x38,
        lf64 = 0x39,
        si8 = 0x3a,
        si16 = 0x3b,
        si32 = 0x3c,
        sf32 = 0x3d,
        sf64 = 0x3e,
        newfunction = 0x40,
        call = 0x41,
        construct = 0x42,
        callmethod = 0x43,
        callstatic = 0x44,
        callsuper = 0x45,
        callproperty = 0x46,
        returnvoid = 0x47,
        returnvalue = 0x48,
        constructsuper = 0x49,
        constructprop = 0x4a,
        callproplex = 0x4c,
        callsupervoid = 0x4e,
        callpropvoid = 0x4f,
        sxi1 = 0x50,
        sxi8 = 0x51,
        sxi16 = 0x52,
        applytype = 0x53,
        newobject = 0x55,
        newarray = 0x56,
        newactivation = 0x57,
        newclass = 0x58,
        getdescendants = 0x59,
        newcatch = 0x5a,
        findpropstrict = 0x5d,
        findproperty = 0x5e,
        finddef = 0x5f,
        getlex = 0x60,
        setproperty = 0x61,
        getlocal = 0x62,
        setlocal = 0x63,
        getglobalscope = 0x64,
        getscopeobject = 0x65,
        getproperty = 0x66,
        getouterscope = 0x67,
        initproperty = 0x68,
        deleteproperty = 0x6a,
        getslot = 0x6c,
        setslot = 0x6d,
        getglobalslot = 0x6e,
        setglobalslot = 0x6f,
        convert_s = 0x70,
        esc_xelem = 0x71,
        esc_xattr = 0x72,
        convert_i = 0x73,
        convert_u = 0x74,
        convert_d = 0x75,
        convert_b = 0x76,
        convert_o = 0x77,
        checkfilter = 0x78,
        coerce = 0x80,
        coerce_b = 0x81,
        coerce_a = 0x82,
        coerce_i = 0x83,
        coerce_d = 0x84,
        coerce_s = 0x85,
        astype = 0x86,
        astypelate = 0x87,
        coerce_u = 0x88,
        coerce_o = 0x89,
        negate = 0x90,
        increment = 0x91,
        inclocal = 0x92,
        decrement = 0x93,
        declocal = 0x94,
        tof = 0x95,
        not = 0x96,
        bitnot = 0x97,
        add = 0xa0,
        subtract = 0xa1,
        multiply = 0xa2,
        divide = 0xa3,
        modulo = 0xa4,
        lshift = 0xa5,
        rshift = 0xa6,
        urshift = 0xa7,
        bitand = 0xa8,
        bitor = 0xa9,
        bitxor = 0xaa,
        equals = 0xab,
        strictequals = 0xac,
        lessthan = 0xad,
        lessequals = 0xae,
        greaterthan = 0xaf,
        greaterequals = 0xb0,
        instanceof = 0xb1,
        istype = 0xb2,
        istypelate = 0xb3,
        inside = 0xb4,
        increment_i = 0xc0,
        decrement_i = 0xc1,
        inclocal_i = 0xc2,
        declocal_i = 0xc3,
        negate_i = 0xc4,
        add_i = 0xc5,
        subtract_i = 0xc6,
        multiply_i = 0xc7,
        getlocal0 = 0xd0,
        getlocal1 = 0xd1,
        getlocal2 = 0xd2,
        getlocal3 = 0xd3,
        setlocal0 = 0xd4,
        setlocal1 = 0xd5,
        setlocal2 = 0xd6,
        setlocal3 = 0xd7,
        debug = 0xef,
        debugline = 0xf0,
        debugfile = 0xf1,
        bkptline = 0xf2,
        timestamp = 0xf3,
    }

    internal class OPArgsArray
    {
        private string[] core;

        public OPArgsArray()
        {
            this.core = new string[0x100];
        }

        public string this[ABCOp index]
        {
            get { return this.core[(byte)index]; }
            set { this.core[(byte)index] = value; }
        }
    }

    internal static class OPArgs
    {
        private static bool Initialized = false;
        public static OPArgsArray Args = new OPArgsArray();

        public static void Init()
        {
            if (Initialized)
                return;
            // (M)ultiname (u30)
            // (H) short
            // (B)yte
            // (S)24
            Initialized = true;
            Args[ABCOp.getlocal] = "M";
            Args[ABCOp.getlocal0] = string.Empty;
            Args[ABCOp.getlocal1] = string.Empty;
            Args[ABCOp.getlocal2] = string.Empty;
            Args[ABCOp.getlocal3] = string.Empty;
            Args[ABCOp.setlocal] = "M";
            Args[ABCOp.setlocal0] = string.Empty;
            Args[ABCOp.setlocal1] = string.Empty;
            Args[ABCOp.setlocal2] = string.Empty;
            Args[ABCOp.setlocal3] = string.Empty;
            Args[ABCOp.newactivation] = string.Empty;
            Args[ABCOp.dup] = string.Empty;
            Args[ABCOp.getscopeobject] = "B";
            Args[ABCOp.coerce] = "M";
            Args[ABCOp.coerce_s] = string.Empty;
            Args[ABCOp.setslot] = "M";
            Args[ABCOp.getlex] = "M";
            Args[ABCOp.initproperty] = "M";
            Args[ABCOp.getproperty] = "M";
            Args[ABCOp.callproperty] = "MM";
            Args[ABCOp.setproperty] = "M";
            Args[ABCOp.equals] = string.Empty;
            Args[ABCOp.iftrue] = "S";
            Args[ABCOp.iffalse] = "S";
            Args[ABCOp.ifeq] = "S";
            Args[ABCOp.ifge] = "S";
            Args[ABCOp.ifgt] = "S";
            Args[ABCOp.ifle] = "S";
            Args[ABCOp.iflt] = "S";
            Args[ABCOp.ifne] = "S";
            Args[ABCOp.ifnge] = "S";
            Args[ABCOp.ifngt] = "S";
            Args[ABCOp.ifnle] = "S";
            Args[ABCOp.ifnlt] = "S";
            Args[ABCOp.ifstricteq] = "S";
            Args[ABCOp.ifstrictne] = "S";
            Args[ABCOp.pop] = string.Empty;
            Args[ABCOp.popscope] = string.Empty;
            Args[ABCOp.callpropvoid] = "MM";
            Args[ABCOp.jump] = "S";
            Args[ABCOp.newcatch] = "M";
            Args[ABCOp.swap] = string.Empty;
            Args[ABCOp.kill] = "M";
            Args[ABCOp.findproperty] = "M";
            Args[ABCOp.findpropstrict] = "M";
            Args[ABCOp.constructprop] = "MM";
            Args[ABCOp.add] = string.Empty;
            Args[ABCOp.subtract] = string.Empty;
            Args[ABCOp.multiply] = string.Empty;
            Args[ABCOp.divide] = string.Empty;
            Args[ABCOp.negate] = string.Empty;
            Args[ABCOp.not] = string.Empty;
            Args[ABCOp.applytype] = "M";
            Args[ABCOp.construct] = "M";
            Args[ABCOp.constructsuper] = "M";
            Args[ABCOp.convert_b] = string.Empty;
            Args[ABCOp.convert_d] = string.Empty;
            Args[ABCOp.convert_i] = string.Empty;
            Args[ABCOp.convert_o] = string.Empty;
            Args[ABCOp.convert_s] = string.Empty;
            Args[ABCOp.convert_u] = string.Empty;
            Args[ABCOp.pushscope] = string.Empty;
            Args[ABCOp.pushfalse] = string.Empty;
            Args[ABCOp.pushtrue] = string.Empty;
            Args[ABCOp.pushnull] = string.Empty;
            Args[ABCOp.pushbyte] = "B";
            Args[ABCOp.pushshort] = "H";
            Args[ABCOp.pushstring] = "M";
            Args[ABCOp.pushint] = "M";
            Args[ABCOp.label] = string.Empty;
            Args[ABCOp.inclocal] = "M";
            Args[ABCOp.inclocal_i] = "M";
            Args[ABCOp.returnvoid] = string.Empty;
            Args[ABCOp.returnvalue] = string.Empty;
        }
    }
}