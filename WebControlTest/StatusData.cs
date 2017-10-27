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
        MSRCommError,
        MSRRead,
        MSRRequestAuth,
        MSRBadRead,
        MSRAuth,
        MSRReject,
        MSRWrongProduct,
        MSRSSelectProduct,
        MSRWash1Selected,
        MSRWash2Selected



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

    public class MyProductItem
    {
        public int Code;
        public double Discount;
        public string DiscountType;
        public MyProductItem(int code,double discount,string type)
        {
            Code = code;
            Discount = discount;
            DiscountType = type;
        }
    }

    public class MSRStatusData
    {
        public int status;
        public string carddata;
        public string DriverName;
        public string ErrorDesc;
        public double Limit;
        public string LimitType;
        public List<MyProductItem> ProductsList;
        //public double Discount;
        //public string DiscountType;
        public bool CPassRequired;
        public bool PINRequired;
        public string PIN;
        public string Reference;
        public MsgLogType msg;
        public int nozz;
        //public int[] ProductsCode;
    }
}
