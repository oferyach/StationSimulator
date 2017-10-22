using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForeFuelSimulator
{
    public enum MsgLogType
    {
        NozzleUP,
        NozzelDown,
        Suspending,
        Resuming,
        TransEnded,
        CheckAuth,
        CheckCPass,
        Authorized,
        CannotAuth,
        CancelAuth,
        StartRefuel,
        OtherCard,
        NewCard,
        Disconnected,
        CommErr,
        WGTConnected,

        //MSR part
        MSRRead,
        MSRRequestAuth,
        MSRBadRead,
        MSRAuth,
        MSRReject,
        MSRWrongProduct,
        MSRSSelectProduct



    };
    public class StatusData
    {
        public int status;
        public string carddata;
        public int nozz;

        public int status2;
        public string carddata2;
        public int nozz2;
    }


    public class PumpUpdateData
    {
        public double vol;
        public double money;
        public string plate;
        public double PPV;
    }

    public class MSRStatusData
    {
        public int status;
        public string carddata;
        public string DriverName;
        public string ErrorDesc;
        public double Limit;
        public double Discount;
        public string Reference;
        public MsgLogType msg;
        public int nozz;
        public int[] ProductsCode;
    }
}
