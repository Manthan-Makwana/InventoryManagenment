using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Practical08.Models;

namespace Practical08.Controllers
{
    public class ProductController : Controller
    {
        string cs = ConfigurationManager
            .ConnectionStrings["InventoryDB"].ConnectionString;

        // GET: Product/Index
        [HttpGet]
        public ActionResult Index()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Products";
                SqlCommand cmd = new SqlCommand(query, con);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    products.Add(new Product
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"] == DBNull.Value
                            ? "" : dr["Description"].ToString(),
                        Price = Convert.ToDecimal(dr["Price"]),
                        Quantity = Convert.ToInt32(dr["Quantity"]),
                        Category = dr["Category"] == DBNull.Value
                            ? "" : dr["Category"].ToString()
                    });
                }
            }

            return View(products);
        }

        // GET: Product/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product p)
        {
            if (!ModelState.IsValid)
                return View(p);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"INSERT INTO Products
                    (Name, Description, Price, Quantity, Category)
                    VALUES
                    (@Name, @Description, @Price, @Quantity, @Category)";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                    .Value = p.Name;

                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500)
                    .Value = (object)p.Description ?? DBNull.Value;

                cmd.Parameters.Add("@Price", SqlDbType.Decimal)
                    .Value = p.Price;
                cmd.Parameters["@Price"].Precision = 10;
                cmd.Parameters["@Price"].Scale = 2;

                cmd.Parameters.Add("@Quantity", SqlDbType.Int)
                    .Value = p.Quantity;

                cmd.Parameters.Add("@Category", SqlDbType.NVarChar, 100)
                    .Value = (object)p.Category ?? DBNull.Value;

                con.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // GET: Product/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            Product p = GetProduct(id);

            if (p == null)
                return HttpNotFound();

            return View(p);
        }

        // POST: Product/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product p)
        {
            if (!ModelState.IsValid)
                return View(p);

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = @"UPDATE Products SET
                    Name=@Name,
                    Description=@Description,
                    Price=@Price,
                    Quantity=@Quantity,
                    Category=@Category
                    WHERE Id=@Id";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = p.Id;
                cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100)
                    .Value = p.Name;
                cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 500)
                    .Value = (object)p.Description ?? DBNull.Value;

                cmd.Parameters.Add("@Price", SqlDbType.Decimal)
                    .Value = p.Price;
                cmd.Parameters["@Price"].Precision = 10;
                cmd.Parameters["@Price"].Scale = 2;

                cmd.Parameters.Add("@Quantity", SqlDbType.Int)
                    .Value = p.Quantity;

                cmd.Parameters.Add("@Category", SqlDbType.NVarChar, 100)
                    .Value = (object)p.Category ?? DBNull.Value;

                con.Open();

                if (cmd.ExecuteNonQuery() == 0)
                    return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        // GET: Product/Delete/5
        [HttpGet]
        public ActionResult Delete(int id)
        {
            Product p = GetProduct(id);

            if (p == null)
                return HttpNotFound();

            return View(p);
        }

        // POST: Product/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "DELETE FROM Products WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                con.Open();

                if (cmd.ExecuteNonQuery() == 0)
                    return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        // Get one product by ID
        private Product GetProduct(int id)
        {
            Product p = null;

            using (SqlConnection con = new SqlConnection(cs))
            {
                string query = "SELECT * FROM Products WHERE Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@Id", SqlDbType.Int).Value = id;

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    p = new Product
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Name = dr["Name"].ToString(),
                        Description = dr["Description"] == DBNull.Value
                            ? "" : dr["Description"].ToString(),
                        Price = Convert.ToDecimal(dr["Price"]),
                        Quantity = Convert.ToInt32(dr["Quantity"]),
                        Category = dr["Category"] == DBNull.Value
                            ? "" : dr["Category"].ToString()
                    };
                }
            }

            return p;
        }
    }
}
