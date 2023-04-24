using Club.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace Club.Controllers
{
    public class ClubController : Controller
    {
        // in Program.cs you will find this 
        //    app.MapControllerRoute(
        //        name: "default",
        //pattern: "{controller=Home}/{action=Index}/{id?}");
        // Change controller=Home to controller=Club

        public IActionResult Index()
        {
            // right click on Index() ^ above and add view
            // select option 2 Raser View -  this will do alot of the heavy lifting
            // For the first time:            
            //view name =  index
            //template = List
            //model class = ClubInfo (Club.models)
            //Do this 4 more times with Edit, Create, Details, Delete
            //change the view name To Edit, Create, Details, Delete
            // change the template name To Edit, Create, Details, Delete
            return View(GetInfo());
            // add GetInfo() ^^ as a parameter to the the return View() when you make the function;
        }
        // in the club folder go to index.cshtml change primary key to ID
        // @Html.ActionLink("Edit", "Edit", new {  /*id=item.PRIMARYKEY*/  }) |
        //@Html.ActionLink("Details", "Details", new { /* id=item.PRIMARYKEY*/  }) |
        // @Html.ActionLink("Delete", "Delete", new {  /*id=item.PRIMARYKEY*/  })
        [HttpGet]
        // the purpose of [HTTPGET] is to get database data and pass it to the view
        public List<ClubInfo> GetInfo()
        {
            //First we need to create an empty list that will hold Club Member Objects

            List<ClubInfo> listObj = new List<ClubInfo>();
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            //then a connection object with our connection string
            using (SqlConnection connObj = new SqlConnection(connString))
            {
                //open connection string
                connObj.Open();
                string queryString = "SELECT * FROM ClubMember";
                using (SqlCommand cmdList = new SqlCommand(queryString, connObj))
                {
                    // use a SQLcommand to make an sql statement
                    using (SqlDataReader reader = cmdList.ExecuteReader())
                    {
                        //Use data reader to read
                        while (reader.Read())
                        {
                            //add all the member details to an object
                            ClubInfo clubInfo = new ClubInfo();
                            clubInfo.ID = Convert.ToInt32(reader["ID"]);
                            clubInfo.FirstName = reader["FirstName"].ToString();
                            clubInfo.LastName = reader["LastName"].ToString();
                            clubInfo.DOB = reader["DOB"].ToString();
                            clubInfo.Rank = reader["Rank"].ToString();
                            listObj.Add(clubInfo);// add each object to a list
                        }
                        reader.Close();
                    }
                }
                connObj.Close();
                //return the list 
                return listObj;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            //1.Begin by creating a new method called "Create" with an HTTP GET attribute.This method will be used to display a view to the user.
            //2.Inside the "Create" method, return a "View" object that displays an empty form for creating a new club member.
            //this returns our View() for create, make sure to return View()
            return View();
        }

        [HttpPost]
        public ActionResult Create(ClubInfo clubMember)
        //6.Inside the HTTP POST method, create an instance of the "ClubMembers" class and pass the form data submitted by the user to this object.
        //3.Create a new method called "Create" with an HTTP POST attribute.This method will be used to handle the form submission.
        {
            //4.Define a connection string to connect to the database.This includes the name of the database, the server name, and security credentials to access the database.
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            //5.Define an SQL query string that inserts a new row into the "ClubMembers" table with the form data submitted by the user.
            //string queryString = $"INSERT INTO ClubMember (ID,FirstName,LastName,Rank,DOB) VALUES ('{clubMember.ID}','{clubMember.FirstName}','{clubMember.LastName}','{clubMember.Rank}','{clubMember.DOB}')";
            string queryString = $"INSERT INTO ClubMember (ID,FirstName,LastName,Rank,DOB) VALUES (@id,@firstName,@lastName,@rank,@DOB)";
            using SqlConnection connObj = new SqlConnection(connString);
            {
                //7. Open a connection to the database using the connection string.
                connObj.Open();
                //8. Create a command object called "cmd" that will be used to execute the SQL query against the database.
                using SqlCommand cmdCreate = new SqlCommand(queryString, connObj);
                {
                    //------add query string with parameters
                    cmdCreate.Parameters.AddWithValue("@id", clubMember.ID);                    
                    cmdCreate.Parameters.AddWithValue("@firstName", clubMember.FirstName);
                    cmdCreate.Parameters.AddWithValue("@lastName", clubMember.LastName);
                    cmdCreate.Parameters.AddWithValue("@rank",clubMember.Rank);
                    cmdCreate.Parameters.AddWithValue("@DOB", clubMember.DOB);
                    //9. Set the command object's connection property to the open connection to the database.
                    cmdCreate.Connection = connObj;
                    //connObj.Open();
                    //10. Execute the SQL query using the command object to insert the new club member data into the database.
                    cmdCreate.ExecuteNonQuery();
                    //11. Close the database connection.
                    connObj.Close();
                }
            }
            //12. Return a "View" object that displays the "Index" view and the updated data retrieved from the database by calling the "GetInfoFromDB" method.
            return View("Index", GetInfo());
        }
        //To create an Edit action method in an ASP.NET MVC project using SQL Client:
        [HttpGet]
        //1. Add a new HttpGet action method to the controller class, accepting an id parameter.
        public ActionResult Edit(int id)
        {
            //2. Inside the HttpGet action method, create a connection string to connect to the database.
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using SqlConnection connObj = new SqlConnection(connString);
            {
                connObj.Open();
                //3. Create a SQL query string to select the record with the specified id value from the ClubMembers table.
                string stringQuery = $"SELECT * FROM CLubMember WHERE ID=@id";
                //4. Create a new ClubMembers object to store the retrieved record data.
                ClubInfo clubInfo = new ClubInfo();
                //5. Using a SqlConnection object, connect to the database and execute the SQL query string to retrieve the record with the specified id.
                using SqlCommand cmdEdit = new SqlCommand(stringQuery, connObj);
                {
                    cmdEdit.Parameters.AddWithValue("@id", id);
                    //6. Using a SqlDataReader object, read the retrieved record data and store it in the ClubMembers object.
                    using SqlDataReader reader = cmdEdit.ExecuteReader();
                    {
                        if (reader.Read())
                        {
                            clubInfo.ID = Convert.ToInt32(reader["ID"]);
                            clubInfo.FirstName = reader["FirstName"].ToString();
                            clubInfo.LastName = reader["LastName"].ToString();
                            clubInfo.DOB = reader["DOB"].ToString();
                            clubInfo.Rank = reader["Rank"].ToString();

                            //ClubInfo viewModel = new ClubInfo
                            //{
                            //    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                            //    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            //    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            //    Rank=reader.GetString(reader.GetOrdinal("Rank")),
                            //    DOB=reader.GetString(reader.GetOrdinal("DOB"))
                            //};
                            //return View(viewModel);
                            return View(clubInfo);
                        }
                        reader.Close();
                    }
                }

                //7. Return the ClubMembers object to the corresponding view using the View method.
                connObj.Close();
                return View("Edit", Edit(clubInfo.ID));

                // this is the same idea as the first create function , just a different purpose.
                //    // we get the edit view so we can see the page (this is done by [HTTPGET])
                //    // then we need to make a member object
                //    //then a connection object
                //    //then open the connection
                //    //now make a Command with a query to find the data of the member you want to edit
                //    // Now use a data reader to read the data
                //    // put the data into our member object
                //    //now return the View with your member profile
                //    // When you run your program you will see the input fields filled with your member data

            }

        }


        [HttpPost]
        //To create an Edit HTTP POST action method:
        public ActionResult Edit(ClubInfo clubMember)
        //1. Add a new HttpPost action method to the controller class, accepting an ID and a ClubMembers object parameter.
        {
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            //2. Inside the HttpPost action method, create a connection string to connect to the database.
            using SqlConnection connObj = new SqlConnection(connString);
            {
                connObj.Open();

                //3. Create a SQL query string to update the record with the specified ID value in the ClubMembers table with the data in the ClubMembers object parameter.
                string queryString = $"UPDATE ClubMember SET FirstName=@firstName,LastName=@lastName,Rank=@rank,DOB=@DOB WHERE ID=@id;";
                //4. Using a SqlConnection object, connect to the database and execute the SQL query string to update the record with the specified ID.
                using SqlCommand cmdUpdate = new SqlCommand(queryString, connObj);
                {
                    cmdUpdate.Parameters.AddWithValue("@id", clubMember.ID);
                    cmdUpdate.Parameters.AddWithValue("@firstName", clubMember.FirstName);
                    cmdUpdate.Parameters.AddWithValue("@lastName", clubMember.LastName);
                    cmdUpdate.Parameters.AddWithValue("@rank", clubMember.Rank);
                    cmdUpdate.Parameters.AddWithValue("@DOB", clubMember.DOB);
                    using (SqlDataReader reader = cmdUpdate.ExecuteReader())
                    {
                        //Use data reader to read
                        while (reader.Read())
                        {
                            //add all the member details to an object
                            ClubInfo clubInfo = new ClubInfo();
                            clubInfo.ID = Convert.ToInt32(reader["ID"]);
                            clubInfo.FirstName = reader["FirstName"].ToString();
                            clubInfo.LastName = reader["LastName"].ToString();
                            clubInfo.DOB = reader["DOB"].ToString();
                            clubInfo.Rank = reader["Rank"].ToString();
                        }
                        reader.Close();
                    }
                }
                //then a connection object
                //then open the connection
                //now make a Command with a query to find the data of the member you want to edit
                // Now use a data reader to read the data
                // Execte query
                // redirect to /Club

                connObj.Close();
                return View("index", GetInfo());
                //5. Redirect the user to the Index action method of the controller class.
            }
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            // this is the same idea as the first edit function, But now we just want to delete.
            // we get the Delete view so we can see the page (this is done by [HTTPGET])
            // then we need to make a member object
            //then a connection object
            //then open the connection
            //now make a Command with a query to find the data of the member you want to delete
            // Now use a data reader to read the data
            // put the data into our member object
            //now return the View with your member profile
            // When you run your program you will see the input fields filled with your member data
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using SqlConnection connObj = new SqlConnection(connString);
            {
                connObj.Open();
                string stringQuery = $"SELECT * FROM CLubMember WHERE ID=@id";

                ClubInfo clubInfo = new ClubInfo();

                using SqlCommand cmdSelect = new SqlCommand(stringQuery, connObj);
                {
                    cmdSelect.Parameters.AddWithValue("@id", id);
                    using SqlDataReader reader = cmdSelect.ExecuteReader();
                    {
                        if (reader.Read())
                        {
                            clubInfo.ID = Convert.ToInt32(reader["ID"]);
                            clubInfo.FirstName = reader["FirstName"].ToString();
                            clubInfo.LastName = reader["LastName"].ToString();
                            clubInfo.DOB = reader["DOB"].ToString();
                            clubInfo.Rank = reader["Rank"].ToString();

                            return View(clubInfo);
                        }
                        reader.Close();
                    }
                    connObj.Close();
                }
                return View("Delete", Delete(clubInfo.ID));
            }
        }

        [HttpPost]
        public ActionResult Delete(ClubInfo clubMember)
        {
            //then a connection object
            //then open the connection
            //now make a Command with a query to find the data of the member you want to Delete
            // Now use a data reader to read the data
            // Execte query
            // redirect to /Club
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

            using SqlConnection connObj = new SqlConnection(connString);
            {
                connObj.Open();

                string queryString = $"DELETE FROM ClubMember WHERE ID=@id;";

                using SqlCommand cmdDelete = new SqlCommand(queryString, connObj);
                {
                    cmdDelete.Parameters.AddWithValue("@id", clubMember.ID);
                    using (SqlDataReader reader = cmdDelete.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            ClubInfo clubInfo = new ClubInfo();
                            clubInfo.ID = Convert.ToInt32(reader["ID"]);
                            clubInfo.FirstName = reader["FirstName"].ToString();
                            clubInfo.LastName = reader["LastName"].ToString();
                            clubInfo.DOB = reader["DOB"].ToString();
                            clubInfo.Rank = reader["Rank"].ToString();
                        }
                        reader.Close();
                    }
                }
                connObj.Close();
                return View("index", GetInfo());
            }
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            // this is the same idea as the first edit function, But now we just want to Details.
            // we get the Details view so we can see the page (this is done by [HTTPGET])
            // then we need to make a member object
            ClubInfo member = new ClubInfo();
            //then a connection object
            //then open the connection
            string connString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ClubInfo;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
            using SqlConnection connObj = new SqlConnection(connString);
            {
                connObj.Open();
                string stringQuery = $"SELECT * FROM CLubMember WHERE ID=@id;";
                //now make a Command with a query to find the data of the member you want to See
                // Now use a data reader to read the data
                // put the data into our member object
                using SqlCommand cmdDetail = new SqlCommand(stringQuery, connObj);
                {
                    cmdDetail.Parameters.AddWithValue("@id", id);
                    using SqlDataReader reader = cmdDetail.ExecuteReader();
                    {
                        if (reader.Read())
                        {
                            member.ID = Convert.ToInt32(reader["ID"]);
                            member.FirstName = reader["FirstName"].ToString();
                            member.LastName = reader["LastName"].ToString();
                            member.DOB = reader["DOB"].ToString();
                            member.Rank = reader["Rank"].ToString();

                            //return View(member);
                            //now return the View with your member profile
                        }
                        reader.Close();
                    }
                }
                connObj.Close();
                // When you run your program you will see the your member data
                return View("Details", member);
            }
        }
    }
}








