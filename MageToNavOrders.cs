using System;
using System.Collections;
using MNC_Product_Sync.MagentoConnectService;
using System.Collections.Generic;
using System.Net;
using ConnectCsharpToMysql;

class Order
{
    private static string _apiUser;
    private static string _apiKey;
    private static string _status;
  
       
    private static directoryRegionEntity[] _stateCodes;

    private static System.Net.Security.RemoteCertificateValidationCallback mIgnoreInvalidCertificates;
    

    public static void MainOrders(string[] args)
    {

        MNC_Product_Sync.Navision_SalesOrder_Service.SalesOrder_PortClient client = new MNC_Product_Sync.Navision_SalesOrder_Service.SalesOrder_PortClient();



        client.ClientCredentials.Windows.ClientCredential.UserName = "Administrator";

        client.ClientCredentials.Windows.ClientCredential.Password = "itree@123";

        client.ClientCredentials.Windows.ClientCredential.Domain = "122.166.222.116";

        DBConnect db = new DBConnect();
        MNC_Product_Sync.MagentoConnectService.PortTypeClient mage_client = new MNC_Product_Sync.MagentoConnectService.PortTypeClient();

        string token_id = null;

        
        try
        {
            ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

             token_id = mage_client.login("webserviceuser", "apikey");
            _stateCodes = mage_client.directoryRegionList(token_id, "US");

            filters filtres = new filters();

            filtres.complex_filter = new complexFilter[]
        {
            new complexFilter()
            {
                //key = "created_at",
                //value = new associativeEntity
                //{
                //    key = "gt",
                //    value = "2015-07-28 08:00:00"
                //}

                key = "status",
                value = new associativeEntity
                {
                    key = "eq",
                    value = "pending"
                }

            }
        };

            salesOrderListEntity[] salesOrders = mage_client.salesOrderList(token_id, filtres);

            Hashtable hsc = db.fetch_Customer1Mapping();


            foreach (salesOrderListEntity orderHeader in salesOrders)
            {

                if (!db.fetch_Order(orderHeader.increment_id.ToString(), "1"))
                {
                    MNC_Product_Sync.Navision_SalesOrder_Service.SalesOrder so = new MNC_Product_Sync.Navision_SalesOrder_Service.SalesOrder();

                    MNC_Product_Sync.Navision_SalesOrder_Service.Sales_Order_Line[] sls = new MNC_Product_Sync.Navision_SalesOrder_Service.Sales_Order_Line[50];
                    salesOrderEntity orderDetail = mage_client.salesOrderInfo(token_id, orderHeader.increment_id);
                    salesOrderAddressEntity billToAddress = orderDetail.billing_address;
                    salesOrderAddressEntity shipToAddress = orderDetail.shipping_address;
                    so.Bill_to_Address = billToAddress.street + " " + billToAddress.region + " " + billToAddress.postcode;
                    so.Ship_to_Address = billToAddress.street + " " + billToAddress.region + " " + billToAddress.postcode;
                    so.Bill_to_Name = billToAddress.firstname + " " + billToAddress.lastname + " ";
                    so.Ship_to_Name = shipToAddress.firstname + " " + shipToAddress.lastname;
                    so.Bill_to_Contact = billToAddress.telephone;
                    so.Sell_to_Customer_No = hsc[orderDetail.customer_id].ToString();
                    so.Ship_to_Contact = billToAddress.telephone;
                    so.External_Document_No = orderHeader.increment_id;
                    so.Bill_to_Customer_No = hsc[orderDetail.customer_id].ToString();
                    so.Sell_to_Customer_Name = orderHeader.customer_firstname + " " + orderHeader.customer_lastname;
                    so.Order_Date = DateTime.Parse(orderHeader.created_at);
                    so.VAT_Bus_Posting_Group = "FOREIGN";



                    int i = 0;

                    foreach (salesOrderItemEntity li in orderDetail.items)
                    {
                        if (!db.fetch_Order(orderHeader.increment_id.ToString(), (i + 1).ToString()))
                        {
                            MNC_Product_Sync.Navision_SalesOrder_Service.Sales_Order_Line sl = new MNC_Product_Sync.Navision_SalesOrder_Service.Sales_Order_Line();
                            sl.Quantity = decimal.Parse(li.qty_ordered);
                            sl.VAT_Prod_Posting_Group = "DOMESTIC";
                            sl.No = li.sku;
                            sl.Description = li.name;
                            sl.Unit_Price = decimal.Parse(li.base_price);
                            sl.Line_Amount = decimal.Parse(li.base_row_total);
                            sl.TypeSpecified = true;
                            sl.Sales_Header_Repl = decimal.Parse(li.base_price).ToString();
                            sl.QuantitySpecified = true;
                            sl.Type = MNC_Product_Sync.Navision_SalesOrder_Service.Type.Item;
                            sl.Line_No = i + 1;
                            sls[i] = sl;


                            i = i + 1;
                            db.InsertOrderMapping(so.No, sl.Line_No.ToString(), orderHeader.increment_id, sl.Line_No.ToString(), orderHeader.customer_id);
                        }
                    }
                    if (!db.fetch_Order(orderHeader.increment_id.ToString(), (i + 1).ToString()))
                    {
                        try { 
                        so.SalesLines = sls;
                        client.Create(ref so);
                            if (so.No != null)
                            {
                                db.UpdateOrder(orderHeader.increment_id, so.No);
                                db.InsertLog("Order", so.No, "Navision Order Number", "SUCCESS");
                            }
                        }
                        catch(Exception ex)
                        {
                            db.deleteOrder(orderHeader.increment_id);
                            db.InsertLog("Order", orderHeader.increment_id, ex.Message, "FAILED");
                        }
                    }
                   
                }
             

            }
        }
        catch (Exception ex)
        {

            db.InsertLog("Order",  "" , ex.Message.ToString(), "FAILED");
            MNC_Product_Sync.ErrorLog errLog = new MNC_Product_Sync.ErrorLog();

            errLog.LogError("C:\\MNC_Logs", "Order : " + ex.Message);

        }
        finally
        {
            mage_client.endSession(token_id);
        }
    }




    //public static filter addFilter(filters filtresIn, string key, string op, string value)
    //{
    //    filters filtres = filtresIn;
    //    if (filtres == null)
    //        filtres = new filters();

    //    complexFilter compfiltres = new complexFilter();
    //    compfiltres.key = key;
    //    associativeEntity ass = new associativeEntity();
    //    ass.key = op;
    //    ass.value = value;
    //    compfiltres.value = ass;

    //    List<complexFilter> tmpLst;
    //    if (filtres.complex_filter != null)
    //        // tmpLst = filtres.complex_filter.ToList();
    //        tmpLst = null;
    //    else tmpLst = new List<complexFilter>();

    //    tmpLst.Add(compfiltres);

    //    filtres.complex_filter = compfiltres;

    //    return filtres;
    //}
    private static string getStateAbbreviation(string region_id)
    {
        foreach (directoryRegionEntity _stateCode in _stateCodes)
        {
            if (int.Parse(_stateCode.region_id) == int.Parse(region_id))
            {
                return _stateCode.code;
            }
        }
        return "";
    }
}