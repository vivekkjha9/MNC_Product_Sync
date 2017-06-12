using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MNC_Product_Sync.MagentoConnectService;
using ConnectCsharpToMysql;
using System.Collections;


namespace MNC_Product_Sync
{
    class NavToMageProducts
    {

                
        static void MainPr(string[] args)
        {

            try
            {

                Navision_ItemCategory_Service.ItemCategories nv_Item_Cat = new Navision_ItemCategory_Service.ItemCategories();          

                Navision_ItemList_Service.ItemList_PortClient client = new Navision_ItemList_Service.ItemList_PortClient();

                Navision_ItemList_Service.ItemList nv_ItemList = new Navision_ItemList_Service.ItemList();

                Navision_ItemList_Service.ItemList[] nv_ItemLists;

                client.ClientCredentials.Windows.ClientCredential.UserName = "Administrator";               

                client.ClientCredentials.Windows.ClientCredential.Password = "itree@123";

                client.ClientCredentials.Windows.ClientCredential.Domain = "122.166.222.116";              

                Navision_ItemList_Service.ItemList_Filter f = new Navision_ItemList_Service.ItemList_Filter();               

                nv_ItemLists = client.ReadMultiple(new Navision_ItemList_Service.ItemList_Filter[] { f }, null, 10000);

                MagentoConnectService.PortTypeClient mage_client = new MagentoConnectService.PortTypeClient();

                string token_id = mage_client.login("webserviceuser", "apikey");

                DBConnect db = new DBConnect();

                Hashtable hs = db.fetch_CategoryMapping();

                Hashtable hsp = db.fetch_ProductMapping();

                for (int i = 0; i < nv_ItemLists.Length; i++)
                {

                    MagentoConnectService.catalogProductCreateEntity mage_Product = new MagentoConnectService.catalogProductCreateEntity();


                    try
                    {
                        mage_Product.description = nv_ItemLists[i].Description;
                        mage_Product.name = nv_ItemLists[i].Description;
                        mage_Product.weight = "1";         

                        mage_Product.price = nv_ItemLists[i].Last_Direct_Cost.ToString();
                        mage_Product.short_description = nv_ItemLists[i].Search_Description;
                        string mage_cat_id = null;                       

                        if (nv_ItemLists[i].Item_Category_Code != null)
                        {
                            mage_cat_id = (string)hs[nv_ItemLists[i].Item_Category_Code];
                        }

                        string[] array = { mage_cat_id };
                        mage_Product.category_ids = array;
                        mage_Product.status = "1";
                        mage_Product.visibility = "4";
                        String productType = "simple";

                        var stock = new catalogInventoryStockItemUpdateEntity
                        {
                             is_in_stockSpecified = true,
                             is_in_stock = 1,
                             qty = nv_ItemLists[i].Inventory.ToString()
                        };

                        mage_Product.stock_data = stock;
                        int productid = 0;
                        try
                        {
                            if (db.fetch_Product(nv_ItemLists[i].No))
                            {
                                mage_client.catalogProductUpdate(token_id, (string)hsp[nv_ItemLists[i].No], mage_Product, "default",productType);
                            }
                            else
                            {
                                productid = mage_client.catalogProductCreate(token_id, productType, "4", nv_ItemLists[i].No, mage_Product, "default");
                                db.InsertProductMapping(nv_ItemLists[i].No, productid.ToString(), nv_ItemLists[i].No);
                                db.InsertLog("Product", nv_ItemLists[i].No, "Navision Product Code", "SUCCESS");
                            }
                        }
                        catch (Exception ex)
                        {
                            db.InsertLog("Product", nv_ItemLists[i].No, ex.ToString(), "FAILED");
                        }


                       

                    }
                    catch (Exception ex)
                    {

                        db.InsertLog("Product", nv_ItemLists[i].No, ex.ToString(), "FAILED");
                    }
                }

            }
            catch (Exception ex)
            {
             MNC_Product_Sync.ErrorLog errLog = new MNC_Product_Sync.ErrorLog();            
             errLog.LogError("C:\\MNC_Logs","Product " + ex.Message);
            } 
        }      

       
    }
}
