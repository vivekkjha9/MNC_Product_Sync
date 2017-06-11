using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.IO;
//Add MySql Library
using MySql.Data.MySqlClient;
using System.Collections;

namespace ConnectCsharpToMysql
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "66.199.235.83";
            database = "magnaxtc_mage542";
            uid = "magnaxtc_mage542";
            password = "Pass@w0rd123";

            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "port=3306";

            connection = new MySqlConnection(connectionString);
        }


        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:

                        break;

                    case 1045:

                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {

                return false;
            }
        }




        //Insert statement
        public void InsertLog(string objecttype, string objectvalue, string remarks, string status)
        {
            remarks.Replace("'", "''");
            string query = "INSERT INTO mgme_navision_integrationauditlog_navintegrationauditlog (objecttype, objectvalue,remarks,status,datetime) VALUES('" + objecttype + "','" + objectvalue + "','" + remarks + "','" + status + "','" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss") + "')";

            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();



                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
        }

        public void InsertProductMapping(string productsku, string magentoproductid, string navisionproductid)
        {

            string query = "INSERT INTO mgme_navision_productmapping_navisionproductmapping (productsku, magentoproductid,navisionproductid,createdat,needsync) VALUES('" + productsku + "'," + magentoproductid + ",'" + navisionproductid + "','" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss") + "'," + true + ")";

            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }
            }
        }


        public void InsertProductCatMapping(string navisioncategoryid, string magentocategoryid, string categoryname)
        {

            string query = "INSERT INTO mgme_navision_categorymapping_navisioncategorymapping (categoryname,magentocategoryid,navisioncategoryid,createdat) VALUES('" + categoryname + "'," + magentocategoryid + ",'" + navisioncategoryid + "','" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss") + "')";

            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }
            }
        }

        public void UpdateOrder(string magentoorder,string navisionorder)
        {
            string query = "update mgme_navision_ordermapping_navisionordermapping set navisionordernumber='" + navisionorder + "' where ordernumber ='" + magentoorder + "'";

            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }
            }


        }

        public void InsertOrderMapping(string navisionordernumber,string navisionorderlineid, string magentoorderid,string magentoorderlineid, string navisioncustomerid)
        {
            string query = "INSERT INTO mgme_navision_ordermapping_navisionordermapping (ordernumber,magentoorderlineid,navisionordernumber,navisionorderlineid,navisioncustomerid,createdat) VALUES('" + magentoorderid + "','" + magentoorderlineid +  "','" + navisionordernumber + "','" + navisionorderlineid + "','" + navisioncustomerid + "','" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss") + "')";

            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }
            }


        }


        public void InsertCustomerMapping(string navisioncustnumber, string magentocustid, string custemail)
        {

            custemail.Replace("'", "''");
            string query = "INSERT INTO mgme_navision_customermapping_navisioncustomermapping (customeremail,magentocustomerid,navisioncustomerid,createdat) VALUES('" + custemail + "','" + magentocustid + "','" + navisioncustnumber + "','" + DateTime.Now.ToString("yyyy-MM-dd h:mm:ss") + "')";

            //open connection
            if (this.OpenConnection() == true)
            {
                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    cmd.ExecuteNonQuery();


                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }
            }


        }
        public System.Collections.Hashtable fetch_CustomerMapping()
        {
            string query = "select navisioncustomerid,magentocustomerid from mgme_navision_customermapping_navisioncustomermapping";
            //DBConnect db = new DBConnect();

            Hashtable ret = null;
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                    ret = new Hashtable(21);


                    while (reader.Read())
                    {

                        ret.Add(reader[0].ToString(), reader[1].ToString());

                    }

                    //return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
        }


        public System.Collections.Hashtable fetch_PendingShipments()
        {
            string query = "select navisionordernumber,navisionorderlineid,ordernumber,magentoorderlineid from mgme_navision_ordermapping_navisionordermapping where navisionshipmentid =''";
            //DBConnect db = new DBConnect();

            Hashtable ret = null;
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                    ret = new Hashtable(21);


                    while (reader.Read())
                    {

                        ret.Add(reader[0].ToString()+"/"+ reader[1].ToString() , reader[2].ToString() + "/" + reader[3].ToString());

                    }

                    //return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
        }

        public System.Collections.Hashtable fetch_PendingInvoices()
        {
            string query = "select navisionordernumber,navisionorderlineid,ordernumber,magentoorderlineid from mgme_navision_ordermapping_navisionordermapping where navisioninvoiceid =''";
            //DBConnect db = new DBConnect();

            Hashtable ret = null;
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                    ret = new Hashtable(21);


                    while (reader.Read())
                    {

                        ret.Add(reader[0].ToString() + "/" + reader[1].ToString()  , reader[2].ToString() + "/" + reader[3].ToString());

                    }

                    //return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
        }


        public System.Collections.Hashtable fetch_Customer1Mapping()
        {
            string query = "select navisioncustomerid,magentocustomerid from mgme_navision_customermapping_navisioncustomermapping";
            //DBConnect db = new DBConnect();

            Hashtable ret = null;
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                    ret = new Hashtable(21);


                    while (reader.Read())
                    {

                        ret.Add(reader[1].ToString(), reader[0].ToString());

                    }

                    //return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
        }



        public System.Collections.Hashtable fetch_CategoryMapping()
        {
            string query = "select categoryname,magentocategoryid from mgme_navision_categorymapping_navisioncategorymapping";
            //DBConnect db = new DBConnect();

            Hashtable ret = null;
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                    ret = new Hashtable(21);


                    while (reader.Read())
                    {

                        ret.Add(reader[0].ToString(), reader[1].ToString());

                    }

                    //return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
        }

        public System.Collections.Hashtable fetch_ProductMapping()
        {
            string query = "select productsku,magentoproductid from mgme_navision_productmapping_navisionproductmapping";
            //DBConnect db = new DBConnect();

            Hashtable ret = null;
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                    ret = new Hashtable(1000);


                    while (reader.Read())
                    {

                        ret.Add(reader[0].ToString(), reader[1].ToString());

                    }

                    //return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
        }

        public Boolean fetch_Product(string sku)
        {
            string query = "select productsku from mgme_navision_productmapping_navisionproductmapping where productsku='" + sku + "'";
            //DBConnect db = new DBConnect();
            bool ret = false;
           
            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();



                   
                    if (reader.HasRows)
                    {
                        ret= true;
                    }
                    else
                    {
                        ret= false;
                    }



                    return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;
           
           
        }

        public Boolean fetch_OnlineOrder(string navisionorderno)
        {
            string query = "select ordernumber from mgme_navision_ordermapping_navisionordermapping where 	navisionordernumber='" + navisionorderno + "'";
            //DBConnect db = new DBConnect();
            bool ret = false;

            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();




                    if (reader.HasRows)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }



                    return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;


        }

        public Boolean fetch_Order(string magentoorderid,string magentoorderlineid)
        {
            string query = "select ordernumber from mgme_navision_ordermapping_navisionordermapping where ordernumber='" + magentoorderid + "' and magentoorderlineid='" + magentoorderlineid + "'";
            
            bool ret = false;

            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();




                    if (reader.HasRows)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }



                    return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;


        }
        public Boolean fetch_Customer(string email)
        {
        
            email.Replace("'", "''");
           // email.Replace("`", "''");

            string query = "select customeremail from mgme_navision_customermapping_navisioncustomermapping where navisioncustomerid='" + email + "'";
            //DBConnect db = new DBConnect();
            bool ret = false;

            //open connection
            if (this.OpenConnection() == true)
            {

                try
                {
                    //create command and assign the query and connection from the constructor
                    MySqlCommand cmd = new MySqlCommand(query, connection);

                    //Execute command
                    MySqlDataReader reader = cmd.ExecuteReader();




                    if (reader.HasRows)
                    {
                        ret = true;
                    }
                    else
                    {
                        ret = false;
                    }



                    return ret;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    //close connection
                    this.CloseConnection();
                }

            }
            return ret;


        }

    }
}
