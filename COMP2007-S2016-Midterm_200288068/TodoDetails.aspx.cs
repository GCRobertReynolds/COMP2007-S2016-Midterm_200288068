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
    public partial class TodoDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetToDo();
            }
        }

        protected void GetToDo()
        {
            //Populate the form with existing data from the database
            int toDoID = Convert.ToInt32(Request.QueryString["TodoID"]);

            //Connect to the Entity Framework Database
            using (TodoConnection db = new TodoConnection())
            {
                //Populate a student object instance with the StudentID from the URL Param
                Todo updatedStudent = (from student in db.Todos
                                          where student.TodoID == toDoID
                                          select student).FirstOrDefault();

                //Map the student properties to the from controls
                if (updatedStudent != null)
                {
                    TodoNameTextBox.Text = updatedStudent.TodoName;
                    TodoNotesTextBox.Text = updatedStudent.TodoNotes;
                }
            }
        }

        protected void CancelButton_Click(object sender, EventArgs e)
        {
            {
                //Redirect
                Response.Redirect("~/ToDoList.aspx");
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            //Use Entity Framework to connect to the server
            using (TodoConnection db = new TodoConnection())
            {
                //Use the Student model to create a new student object and
                //Save a new record

                //Student newStudent = new Student();
                Todo newTodo = new Todo();

                int StudentID = 0;

                if (Request.QueryString.Count > 0) //Our URL has a StudentID in it
                {
                    //Get the StudentID from the URL
                    StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

                    //Get the current student from the Entity Framework Database
                    newTodo = (from student in db.Todos
                                  where student.TodoID == StudentID
                                  select student).FirstOrDefault();
                }

                //Add data to the new student record
                newTodo.TodoName = TodoNameTextBox.Text;
                newTodo.TodoNotes = TodoNotesTextBox.Text;

                //Use LINQ/ADO.NET to Add/Insert new student into the database
                if (StudentID == 0)
                {
                    db.Todos.Add(newTodo);
                }


                //Save our changes (also updates and inserts)
                db.SaveChanges();

                //Redirect back to the updated students page
                Response.Redirect("~/TodoList.aspx");
            }
        }
    }
}