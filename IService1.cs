using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OOP_Projekt6_WebService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here

        [OperationContract]
        bool AddShow(DateTime dateTime, string name);

        [OperationContract]
        bool DeleteShow(DateTime dateTime);

        [OperationContract]
        Dictionary<int, bool?> GetSeatingInfo(DateTime dateTime);

        [OperationContract]
        Dictionary<DateTime, string> GetShows();

        [OperationContract]
        Dictionary<DateTime, string> GetShowsByName(string name);

        [OperationContract]
        Dictionary<DateTime, string> GetShowsByDate(DateTime date);

        [OperationContract]
        bool MakeReservation(DateTime dateTime, int seat);

        [OperationContract]
        bool CancelReservation(DateTime dateTime, int seat);
    }


    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
