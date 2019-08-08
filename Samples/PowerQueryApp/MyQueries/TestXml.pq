let
    Source = Xml.Tables(File.Contents("C:\Temp\PowerQueryNet\Samples\PowerQueryApp\MyFiles\taPMTransactionInsert.xml")),
    Table0 = Source{0}[Table],
    Table = Table0{1}[Table],
    #"Changed Type" = Table.TransformColumnTypes(Table,{{"BACHNUMB", type text}, {"VCHNUMWK", type text}, {"VENDORID", type text}, {"DOCNUMBR", Int64.Type}, {"DOCTYPE", Int64.Type}, {"DOCAMNT", type text}, {"DOCDATE", type date}, {"PRCHAMNT", type text}, {"CHRGAMNT", type text}, {"PORDNMBR", Int64.Type}, {"CREATEDIST", Int64.Type}})
in
    #"Changed Type"