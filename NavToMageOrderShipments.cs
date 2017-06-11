using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectCsharpToMysql;
using MNC_Product_Sync.MagentoConnectService;

namespace MNC_Product_Sync
{


    class NavToMageOrderShipments
    {
        private static string _apiUser;
        private static string _apiKey;
        private static string _status;
        private static System.Net.Security.RemoteCertificateValidationCallback mIgnoreInvalidCertificates;

        public static void Main(string[] arg)
        {
            MNC_Product_Sync.Navision_Shipment.PostedSalesShipment_PortClient client = new MNC_Product_Sync.Navision_Shipment.PostedSalesShipment_PortClient();

            client.ClientCredentials.Windows.ClientCredential.UserName = "Administrator";

            client.ClientCredentials.Windows.ClientCredential.Password = "itree@123";

            client.ClientCredentials.Windows.ClientCredential.Domain = "122.166.222.116";

            Navision_Shipment.PostedSalesShipment_Filter f = new Navision_Shipment.PostedSalesShipment_Filter();

            Navision_Shipment.PostedSalesShipment[] nv_PostedShipments;

            nv_PostedShipments = client.ReadMultiple(new Navision_Shipment.PostedSalesShipment_Filter[] { f }, null, 10000);

            DBConnect db = new DBConnect();

            MNC_Product_Sync.MagentoConnectService.PortTypeClient mage_client = new MNC_Product_Sync.MagentoConnectService.PortTypeClient();

            string token_id = null;

            // Create the shipment, and add tracking for it. Similar to invoicing, we don't stop for errors.
            try
            {

                Hashtable hsp = db.fetch_PendingShipments();
                token_id = mage_client.login("webserviceuser", "apikey");

                for (int i = 0; i < nv_PostedShipments.Length; i++)
                {
                    if (db.fetch_OnlineOrder(nv_PostedShipments[i].Order_No))
                    {
                        string magorder = (string)hsp[nv_PostedShipments[i].Order_No + "/" + i];
                        string[] mageorders = magorder.Split('/');

                        string shipmentIncrementId = mage_client.salesOrderShipmentCreate(token_id, mageorders[0], null,
                                "Shipment via MagNaxt Connector/Navision Order" + nv_PostedShipments[i].Order_No, 1, 1);

                        associativeEntity[] validCarriers = mage_client.salesOrderShipmentGetCarriers(token_id, mageorders[0]);

                        try
                        {
                            mage_client.salesOrderShipmentAddTrack(token_id, shipmentIncrementId, nv_PostedShipments[i].Shipping_Agent_Code, "Shipment via Magnaxt Connector", nv_PostedShipments[i].Package_Tracking_No);
                            mage_client.salesOrderAddComment(token_id, mageorders[0], "complete", "Order Completed via API", "0");
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }


        }
    }
}
