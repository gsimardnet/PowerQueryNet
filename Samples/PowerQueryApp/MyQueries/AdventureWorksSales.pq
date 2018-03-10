let
    Source = Sql.Database(".\SQL2016"
    	, "AdventureWorks2012"
    	, [Query="SELECT TOP 10 * FROM Sales.SalesOrderHeader"])
in
    Source