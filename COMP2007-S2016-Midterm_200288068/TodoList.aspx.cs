using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//Using statements required to connect to Entity Framework Database
using COMP2007_S2016_Midterm_200288068.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_S2016_Midterm_200288068
{
    public partial class TodoList : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            //if loading the page for the first time, populate the grid
            if (!IsPostBack)
            {
                Session["SortColumn"] = "TodoID"; //Default sort column
                Session["SortDirection"] = "ASC";
                //Get the student data
                this.GetToDo();
            }
        }
        
        protected void GetToDo()
        {
            //Connect to Entity Framework
            using (TodoConnection db = new TodoConnection())
            {
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                //Query the Students Table using Entity Framework and LINQ
                var toDo = (from allToDo in db.Todos
                                select allToDo);

                //Bind the results to the GridView
                TodoGridView.DataSource = toDo.ToList();
                TodoGridView.DataBind();
            }
        }

        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Set the new page size
            TodoGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            //Refresh the grid
            this.GetToDo();
        }

        protected void TodoGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //Store which row was selected
            int selectedRow = e.RowIndex;

            //Get the selected StudentID using the grids DataKey collection
            int TodoID = Convert.ToInt32(TodoGridView.DataKeys[selectedRow].Values["TodoID"]);

            //Use Entity Framework to find the selected student in the DB and remove it
            using (TodoConnection db = new TodoConnection())
            {
                //Create object of the student class and store the query string inside of it
                Todo deletedTodo = (from toDo in db.Todos
                                    where toDo.TodoID == TodoID
                                    select toDo).FirstOrDefault();

                //Remove the selected student from the database
                db.Todos.Remove(deletedTodo);

                //Save changes to database
                db.SaveChanges();

                //Refresh the gridview
                this.GetToDo();
            }
        }

        protected void TodoGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header) //If header row has been clicked
                {
                    LinkButton linkbutton = new LinkButton();

                    for (int index = 0; index < TodoGridView.Columns.Count - 1; index++)
                    {
                        if (TodoGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if (Session["SortDirection"].ToString() == "ASC")
                            {
                                linkbutton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                            }
                            else
                            {
                                linkbutton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                            }

                            e.Row.Cells[index].Controls.Add(linkbutton);
                        }
                    }
                }
            }
        }

        protected void TodoGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            //Get the column to sort by
            Session["SortColumn"] = e.SortExpression;

            //Refresh the grid
            this.GetToDo();

            //Toggle the direction
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }

        protected void TodoGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //Set the new page number
            TodoGridView.PageIndex = e.NewPageIndex;

            //Refresh the grid
            this.GetToDo();
        }
    }
}