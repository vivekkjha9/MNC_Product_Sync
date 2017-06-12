using System;
using System.Collections;
using MNC_Product_Sync.MagentoConnectService;
using System.Collections.Generic;
using System.Net;
using ConnectCsharpToMysql;

namespace MNC_Product_Sync
{
    class NavToMageOrderInvoices
    {
        private static string _apiUser;
        private static string _apiKey;
        private static string _status;
        private static System.Net.Security.RemoteCertificateValidationCallback mIgnoreInvalidCertificates;



        public static void MainInv(string[] args)
        {

            MNC_Product_Sync.Navision_Invoice.PostedSalesInvoice_PortClient client1 = new MNC_Product_Sync.Navision_Invoice.PostedSalesInvoice_PortClient();

            client1.ClientCredentials.Windows.ClientCredential.UserName = "Administrator";

            client1.ClientCredentials.Windows.ClientCredential.Password = "itree@123";

            client1.ClientCredentials.Windows.ClientCredential.Domain = "122.166.222.116";

            Navision_Invoice.PostedSalesInvoice_Filter f1 = new Navision_Invoice.PostedSalesInvoice_Filter();

            Navision_Invoice.PostedSalesInvoice[] nv_PostedInvoices;

            nv_PostedInvoices = client1.ReadMultiple(new Navision_Invoice.PostedSalesInvoice_Filter[] { f1 }, null, 10000);


            DBConnect db = new DBConnect();

            MNC_Product_Sync.MagentoConnectService.PortTypeClient mage_client = new MNC_Product_Sync.MagentoConnectService.PortTypeClient();

            string token_id = null;
            

            try
            {
                Hashtable hso = db.fetch_PendingInvoices();

                token_id = mage_client.login("webserviceuser", "apikey");
                orderItemIdQty[] invoiceQtys = null;
                for (int i = 0; i < nv_PostedInvoices.Length; i++)
                {
                    if (db.fetch_OnlineOrder(nv_PostedInvoices[i].Order_No))
                    {

                        string magorder = (string)hso[nv_PostedInvoices[i].Order_No + "/" + 1];
                        string[] mageorders = magorder.Split('/');
                        salesOrderEntity orderDetail = mage_client.salesOrderInfo(token_id, mageorders[0]);
                        List<orderItemIdQty> invoiceInfo = new List<orderItemIdQty>();
                        foreach (salesOrderItemEntity li in orderDetail.items)
                        {

                            orderItemIdQty invoiceLineItem = new orderItemIdQty();
                            invoiceLineItem.order_item_id = Convert.ToInt32(li.item_id);
                            invoiceLineItem.qty = Convert.ToDouble(li.qty_ordered);
                            invoiceInfo.Add(invoiceLineItem);

                        }
                        invoiceQtys = invoiceInfo.ToArray();

                        /* Create an invoice, and then capture it. Although we are reporting errors, 
                            we don't do anything about them, Nor do we stop processing.
                         */


                        try
                        {
                            string invoiceIncrementId = mage_client.salesOrderInvoiceCreate(token_id, mageorders[0],
                                    invoiceQtys, "Invoiced via API", "1", "1");
                            try
                            {
                               // mage_client.salesOrderInvoiceCapture(token_id, invoiceIncrementId);
                                mage_client.salesOrderAddComment(token_id, mageorders[0], "invoiced", "Order Invoiced via API", "0");
                                db.UpdateInvoice(magorder, invoiceIncrementId + "/" + nv_PostedInvoices[i].No);
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        catch (Exception ex)
                        {


                        }
                    }
                }
                

                    /* Assuming everything went well (which we do ASSume), this order has been Imported into ERP, Invoiced, 
                        Captured, and Shipped Complete.
                     */
                    
                    mage_client.endSession(token_id);
                
            }
            catch (Exception ex)
            {

            }
        
        }
    }
}
