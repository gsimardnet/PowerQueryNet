let
    Source = Excel.Workbook(File.Contents(List2Xlsx), null, true),
    Table1_Table = Source{[Item="Table1",Kind="Table"]}[Data],
    #"Changed Type" = Table.TransformColumnTypes(Table1_Table,{{"ID", Int64.Type}, {"Name", type text}, {"Description", type text}})
in
    #"Changed Type"