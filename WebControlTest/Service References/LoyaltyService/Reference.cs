﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ForeFuelSimulator.LoyaltyService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthResult", Namespace="http://schemas.datacontract.org/2004/07/AuthService")]
    [System.SerializableAttribute()]
    public partial class AuthResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool AllowedField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DiscountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DiscountTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DriverNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorDescField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double LimitField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LimitTypeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool PINRequiredField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PinCodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private ForeFuelSimulator.LoyaltyService.ProductItem[] ProductsListField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ReferenceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool cPassRequiredField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Allowed {
            get {
                return this.AllowedField;
            }
            set {
                if ((this.AllowedField.Equals(value) != true)) {
                    this.AllowedField = value;
                    this.RaisePropertyChanged("Allowed");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Discount {
            get {
                return this.DiscountField;
            }
            set {
                if ((this.DiscountField.Equals(value) != true)) {
                    this.DiscountField = value;
                    this.RaisePropertyChanged("Discount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DiscountType {
            get {
                return this.DiscountTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.DiscountTypeField, value) != true)) {
                    this.DiscountTypeField = value;
                    this.RaisePropertyChanged("DiscountType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DriverName {
            get {
                return this.DriverNameField;
            }
            set {
                if ((object.ReferenceEquals(this.DriverNameField, value) != true)) {
                    this.DriverNameField = value;
                    this.RaisePropertyChanged("DriverName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDesc {
            get {
                return this.ErrorDescField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorDescField, value) != true)) {
                    this.ErrorDescField = value;
                    this.RaisePropertyChanged("ErrorDesc");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Limit {
            get {
                return this.LimitField;
            }
            set {
                if ((this.LimitField.Equals(value) != true)) {
                    this.LimitField = value;
                    this.RaisePropertyChanged("Limit");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LimitType {
            get {
                return this.LimitTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.LimitTypeField, value) != true)) {
                    this.LimitTypeField = value;
                    this.RaisePropertyChanged("LimitType");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool PINRequired {
            get {
                return this.PINRequiredField;
            }
            set {
                if ((this.PINRequiredField.Equals(value) != true)) {
                    this.PINRequiredField = value;
                    this.RaisePropertyChanged("PINRequired");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PinCode {
            get {
                return this.PinCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.PinCodeField, value) != true)) {
                    this.PinCodeField = value;
                    this.RaisePropertyChanged("PinCode");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public ForeFuelSimulator.LoyaltyService.ProductItem[] ProductsList {
            get {
                return this.ProductsListField;
            }
            set {
                if ((object.ReferenceEquals(this.ProductsListField, value) != true)) {
                    this.ProductsListField = value;
                    this.RaisePropertyChanged("ProductsList");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Reference {
            get {
                return this.ReferenceField;
            }
            set {
                if ((object.ReferenceEquals(this.ReferenceField, value) != true)) {
                    this.ReferenceField = value;
                    this.RaisePropertyChanged("Reference");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool cPassRequired {
            get {
                return this.cPassRequiredField;
            }
            set {
                if ((this.cPassRequiredField.Equals(value) != true)) {
                    this.cPassRequiredField = value;
                    this.RaisePropertyChanged("cPassRequired");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ProductItem", Namespace="http://schemas.datacontract.org/2004/07/AuthService")]
    [System.SerializableAttribute()]
    public partial class ProductItem : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private int CodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private double DiscountField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DiscountTypeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Code {
            get {
                return this.CodeField;
            }
            set {
                if ((this.CodeField.Equals(value) != true)) {
                    this.CodeField = value;
                    this.RaisePropertyChanged("Code");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public double Discount {
            get {
                return this.DiscountField;
            }
            set {
                if ((this.DiscountField.Equals(value) != true)) {
                    this.DiscountField = value;
                    this.RaisePropertyChanged("Discount");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DiscountType {
            get {
                return this.DiscountTypeField;
            }
            set {
                if ((object.ReferenceEquals(this.DiscountTypeField, value) != true)) {
                    this.DiscountTypeField = value;
                    this.RaisePropertyChanged("DiscountType");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TransactionCompleteResult", Namespace="http://schemas.datacontract.org/2004/07/AuthService")]
    [System.SerializableAttribute()]
    public partial class TransactionCompleteResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorDescField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool RecivedField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDesc {
            get {
                return this.ErrorDescField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorDescField, value) != true)) {
                    this.ErrorDescField = value;
                    this.RaisePropertyChanged("ErrorDesc");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Recived {
            get {
                return this.RecivedField;
            }
            set {
                if ((this.RecivedField.Equals(value) != true)) {
                    this.RecivedField = value;
                    this.RaisePropertyChanged("Recived");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LoyaltyService.LoyaltyService")]
    public interface LoyaltyService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoyaltyService/GetAuth", ReplyAction="http://tempuri.org/LoyaltyService/GetAuthResponse")]
        ForeFuelSimulator.LoyaltyService.AuthResult GetAuth(string card, string stationcode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoyaltyService/GetAuth", ReplyAction="http://tempuri.org/LoyaltyService/GetAuthResponse")]
        System.Threading.Tasks.Task<ForeFuelSimulator.LoyaltyService.AuthResult> GetAuthAsync(string card, string stationcode);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoyaltyService/TransactionComplete", ReplyAction="http://tempuri.org/LoyaltyService/TransactionCompleteResponse")]
        ForeFuelSimulator.LoyaltyService.TransactionCompleteResult TransactionComplete(string referece, double amount, double volume, int ProductCode, System.DateTime datetime);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/LoyaltyService/TransactionComplete", ReplyAction="http://tempuri.org/LoyaltyService/TransactionCompleteResponse")]
        System.Threading.Tasks.Task<ForeFuelSimulator.LoyaltyService.TransactionCompleteResult> TransactionCompleteAsync(string referece, double amount, double volume, int ProductCode, System.DateTime datetime);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface LoyaltyServiceChannel : ForeFuelSimulator.LoyaltyService.LoyaltyService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LoyaltyServiceClient : System.ServiceModel.ClientBase<ForeFuelSimulator.LoyaltyService.LoyaltyService>, ForeFuelSimulator.LoyaltyService.LoyaltyService {
        
        public LoyaltyServiceClient() {
        }
        
        public LoyaltyServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LoyaltyServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoyaltyServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LoyaltyServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public ForeFuelSimulator.LoyaltyService.AuthResult GetAuth(string card, string stationcode) {
            return base.Channel.GetAuth(card, stationcode);
        }
        
        public System.Threading.Tasks.Task<ForeFuelSimulator.LoyaltyService.AuthResult> GetAuthAsync(string card, string stationcode) {
            return base.Channel.GetAuthAsync(card, stationcode);
        }
        
        public ForeFuelSimulator.LoyaltyService.TransactionCompleteResult TransactionComplete(string referece, double amount, double volume, int ProductCode, System.DateTime datetime) {
            return base.Channel.TransactionComplete(referece, amount, volume, ProductCode, datetime);
        }
        
        public System.Threading.Tasks.Task<ForeFuelSimulator.LoyaltyService.TransactionCompleteResult> TransactionCompleteAsync(string referece, double amount, double volume, int ProductCode, System.DateTime datetime) {
            return base.Channel.TransactionCompleteAsync(referece, amount, volume, ProductCode, datetime);
        }
    }
}
