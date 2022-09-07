Take a look at the Issues.
For some code guidelines - It was entirely build with WPF and MVVM (DevExpress.Mvvm from NuGet).
The Project is separated at 4 layers - View - ViewModel - Business Logic - Data Access. All of them use entities which are avaibale everywhere (you might konw them as "model" as well. I prefer the term entity)
Database access goes through simple ADO.NET and SQL. The used database system is System.Data.SQLite3
