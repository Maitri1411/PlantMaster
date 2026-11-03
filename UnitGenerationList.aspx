
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UnitGenerationList.aspx.cs"
    Inherits="PlantMaster.UnitGenerationList" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unit Generation List</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        * { box-sizing: border-box; }

        body {
            margin: 0;
            padding: 20px;
            font-family: Arial, Helvetica, sans-serif;
            background: #F4F7FB;
            color: #263A55;
        }

        .main-container {
            width: 100%;
            margin: 0 auto;
        }

        .page-title {
            font-size: 23px;
            font-weight: bold;
            color: #17365D;
            margin: 10px 0 18px;
        }

        .toolbar {
            display: flex;
            align-items: center;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 20px;
        }

        .search-box {
            width: 480px;
            max-width: 100%;
            height: 45px;
            padding: 10px 15px;
            border: 1px solid #A8BCD9;
            border-radius: 8px;
            font-size: 16px;
            background: #FFFFFF;
            color: #263A55;
        }

        .btn {
            padding: 10px 20px;
            border: 1px solid #789ACB;
            border-radius: 8px;
            background: #A8BCD9;
            color: #17365D;
            font-size: 15px;
            font-weight: bold;
            cursor: pointer;
        }

        .btn:hover, .grid a:hover {
            background: #91ACD0;
        }

        .grid-container {
            width: 100%;
            overflow-x: auto;
            border-top: 1px solid #A8BCD9;
            padding-top: 18px;
        }

        .grid {
            width: 100%;
            min-width: 950px;
            border-collapse: collapse;
            background: #FFFFFF;
            color: #263A55;
            font-size: 14px;
        }

        .grid th {
            padding: 10px;
            text-align: left;
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #A8BCD9;
        }

        .grid td {
            padding: 8px 10px;
            height: 40px;
            border: 1px solid #D1DFF0;
            vertical-align: middle;
            background: #FFFFFF;
        }

        .grid a {
            display: inline-block;
            margin: 2px;
            padding: 7px 12px;
            border: 1px solid #789ACB;
            border-radius: 7px;
            background: #A8BCD9;
            color: #17365D;
            font-size: 12px;
            text-decoration: none;
            white-space: nowrap;
        }

        .message {
            display: block;
            margin-bottom: 12px;
            color: #263A55;
            font-size: 13px;
            font-weight: bold;
            overflow-wrap: anywhere;
        }

        @media screen and (max-width: 600px) {
            body { padding: 10px; }
            .page-title { font-size: 21px; }
            .toolbar { align-items: stretch; }
            .search-box { width: 100%; }
            .btn { padding: 9px 15px; }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">
    <div class="main-container">

        <div class="page-title">Unit Generation</div>

        <asp:Label ID="lblMessage" runat="server"
            CssClass="message" />

        <div class="toolbar">

            <asp:TextBox ID="txtSearch" runat="server"
                CssClass="search-box"
                placeholder="Search Date, Plant ID or Inverter ID..." />

            
            <asp:Button ID="btnSearch" runat="server"
                Text="Search"
                CssClass="btn"
                OnClick="btnSearch_Click" />

                    <asp:Button
                         ID ="Button1"
                         runat ="server"
                         Text="Report"
                         CssClass="btn"
                       CausesValidation="false"
                      OnClick ="btnReport_Click" />


            <asp:Button ID="btnAddNew" runat="server"
                Text="+ New"
                CssClass="btn"
                OnClick="btnAddNew_Click" />

        </div>

        <div class="grid-container">

            <asp:GridView ID="gvUnitGeneration" runat="server"
                AutoGenerateColumns="False"
                CssClass="grid"
                DataKeyNames="GenerationDate,PlantID,InverterID"
                EmptyDataText="No Unit Generation records found."
                OnRowEditing="gvUnitGeneration_RowEditing"
                OnRowCancelingEdit="gvUnitGeneration_RowCancelingEdit"
                OnRowUpdating="gvUnitGeneration_RowUpdating">

                <Columns>

                    <asp:BoundField
                        DataField="GenerationDate"
                        HeaderText="Generation Date"
                        DataFormatString="{0:dd-MM-yyyy}"
                        HtmlEncode="False"
                        ReadOnly="True" />

                    <asp:BoundField
                        DataField="PlantID"
                        HeaderText="Plant ID"
                        ReadOnly="True" />

                    <asp:BoundField
                        DataField="PGVCL"
                        HeaderText="PGVCL" />

                    <asp:BoundField
                        DataField="GETCO"
                        HeaderText="GETCO" />

                    <asp:BoundField
                        DataField="InverterID"
                        HeaderText="Inverter ID"
                        ReadOnly="True" />

                    <asp:BoundField
                        DataField="Value"
                        HeaderText="Value" />

                    <asp:BoundField
                        DataField="INVTWISE"
                        HeaderText="INVTWISE"
                        ReadOnly="True" />

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <%# Convert.ToBoolean(Eval("IsActive"))
                                ? "Active" : "Inactive" %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:CommandField
                        ShowEditButton="True"
                        EditText="Edit"
                        UpdateText="Update"
                        CancelText="Cancel"
                        HeaderText="Action" />

                </Columns>
            </asp:GridView>

        </div>
    </div>
</form>
</body>
</html>
