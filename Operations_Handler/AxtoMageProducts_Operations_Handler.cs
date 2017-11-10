using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ConnectCsharpToMysql;
using System.Collections;
using AX_CRT_MAge_Connector;
using DynamicsConnectivityValidator;
using AX_CRT_Mage_Connector.MagentoConnectService;

namespace MNC_Product_Sync
{
    class AxtoMageProducts
    {


        static void Mainxds(string[] args)
        {

            try
            {

                var crtManager = new DynamicsRuntimeManager();

                CombinedWriter writer;
                var oldOut = Console.Out;

                try
                {
                    writer = new CombinedWriter("./DynamicsConnectivityValidator.log", false, Encoding.Unicode, 0x400, Console.Out);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Cannot open DynamicsConnectivityValidator.log for writing");
                    Console.WriteLine(e.Message);
                    return;
                }







                AX_CRT_Mage_Connector.MagentoConnectService.PortTypeClient mage_client = new AX_CRT_Mage_Connector.MagentoConnectService.PortTypeClient();

                string token_id = mage_client.login("webserviceuser", "apikey");

                DBConnect db = new DBConnect();

                Hashtable hs = db.fetch_CategoryMapping();

                Hashtable hsp = db.fetch_ProductMapping();

                foreach (DictionaryEntry pair in hs)
                {

                    Microsoft.Dynamics.Commerce.Runtime.PagedResult<Microsoft.Dynamics.Commerce.Runtime.DataModel.SimpleProduct> axProducts = AX_CRT_MAge_Connector.UtilityFunctions.readProducts(crtManager, writer, long.Parse(pair.Key.ToString()));

                    for (int i = 0; i < axProducts.Results.Count; i++)
                    {

                        AX_CRT_Mage_Connector.MagentoConnectService.catalogProductCreateEntity mage_Product = new AX_CRT_Mage_Connector.MagentoConnectService.catalogProductCreateEntity();


                        try
                        {
                            mage_Product.description = axProducts.Results[i].Description;
                            mage_Product.name = axProducts.Results[i].Name;
                            mage_Product.weight = "1";
                            mage_Product.price = axProducts.Results[i].Price.ToString();
                            mage_Product.short_description = axProducts.Results[i].Name;
                            string mage_cat_id = null;                           
                            mage_cat_id = pair.Key.ToString();                    

                            string[] array = { db.fetch_MageCategoryID(long.Parse(mage_cat_id)).ToString() };
                            mage_Product.category_ids = array;
                            mage_Product.status = "1";
                            mage_Product.visibility = "4";
                            String productType = "simple";

                            var stock = new catalogInventoryStockItemUpdateEntity
                            {
                                is_in_stockSpecified = true,
                                is_in_stock = 1,
                                //qty = axProducts.Results[i].Inventory.ToString()
                            };

                            mage_Product.stock_data = stock;
                            int productid = 0;
                            try
                            {
                                if (db.fetch_Product(axProducts.Results[i].ItemId))
                                {
                                    mage_client.catalogProductUpdate(token_id, (string)hsp[axProducts.Results[i].ItemId], mage_Product, "default", productType);
                                }
                                else
                                {
                                    productid = mage_client.catalogProductCreate(token_id, productType, "4", axProducts.Results[i].ItemId, mage_Product, "default");
                                }
                            }
                            catch (Exception ex)
                            {
                                db.InsertLog("Product", axProducts.Results[i].ItemId, ex.ToString(), "FAILED");
                            }

                            if (productid != 0)
                            {
                                db.InsertProductMapping(axProducts.Results[i].ItemId, productid.ToString(), axProducts.Results[i].ItemId);
                                db.InsertLog("Product", axProducts.Results[i].ItemId, "Navision Product Code", "SUCCESS");
                            }

                        }
                        catch (Exception ex)
                        {

                            db.InsertLog("Product", axProducts.Results[i].ItemId, ex.ToString(), "FAILED");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                //MNC_Product_Sync.ErrorLog errLog = new MNC_Product_Sync.ErrorLog();
                //errLog.LogError("C:\\MNC_Logs", "Product " + ex.Message);
            }
        }


    }
}
