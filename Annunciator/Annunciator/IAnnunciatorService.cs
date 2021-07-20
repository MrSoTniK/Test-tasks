using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Annunciator
{
    [ServiceContract(CallbackContract = typeof(IAnnunciatorServiceCallBack))]
    public interface IAnnunciatorService
    {
        [OperationContract]
        int Connect();

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMessage(string message);
    }

    public interface IAnnunciatorServiceCallBack
    {
        [OperationContract(IsOneWay = true)]
        void CallBackMessage(string message);
    }
}
