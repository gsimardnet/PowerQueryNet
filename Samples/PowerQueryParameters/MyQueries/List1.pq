let
    Source = Excel.Workbook(File.Contents(List1Xls), null, true),
    Sheet2 = Source{[Name="Sheet1"]}[Data],
    #"Promoted Headers" = Table.PromoteHeaders(Sheet2, [PromoteAllScalars=true]),
    #"Changed Type" = Table.TransformColumnTypes(#"Promoted Headers",{{"ID", Int64.Type}, {"Name", type text}, {"Description", type text}})
in
    #"Changed Type"