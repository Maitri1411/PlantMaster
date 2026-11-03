
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UnitGeneration.aspx.cs"
    Inherits="PlantMaster.UnitGeneration" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Unit Generation</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        * { box-sizing: border-box; }

        body {
            margin: 0;
            padding: 15px;
            min-height: 100vh;
            font-family: Arial, Helvetica, sans-serif;
            background: #F4F7FB;
            color: #17365D;
        }

        .content { padding: 8px 12px; }

        .detail-card {
            width: 100%;
            max-width: 850px;
            padding: 15px;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 12px;
            box-shadow: 0 6px 15px rgba(30,55,90,.10);
        }

        .detail-heading {
            display: flex;
            align-items: center;
            gap: 8px;
            padding-bottom: 12px;
            border-bottom: 1px solid #A8BCD9;
            margin-bottom: 22px;
        }

        .detail-title {
            font-size: 21px;
            margin: 0;
            font-weight: 600;
        }

        .section-title {
            font-size: 16px;
            font-weight: bold;
            padding-bottom: 9px;
            border-bottom: 1px solid #A8BCD9;
            margin: 22px 0 16px;
        }

        .detail-row {
            display: grid;
            grid-template-columns: 135px minmax(0, 300px);
            align-items: center;
            gap: 12px;
            margin: 15px 10px;
        }

        .detail-label {
            text-align: right;
            font-size: 14px;
        }

        .detail-input {
            width: 100%;
            height: 38px;
            padding: 8px 10px;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            font-size: 14px;
            font-family: Arial, Helvetica, sans-serif;
            color: #17365D;
            background: #FFFFFF;
        }

        .detail-input:focus,
        .inverter-input:focus {
            outline: none;
            border: 2px solid #6E8DBA;
        }

        .readonly { background: #E4EBF5; }

        .table-wrapper {
            width: 100%;
            overflow-x: auto;
            margin-top: 12px;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            background: #FFFFFF;
        }

        .grid {
            width: 100%;
            border-collapse: collapse;
            table-layout: fixed;
            background: #FFFFFF;
        }

        .grid th {
            background: #A8BCD9;
            color: #17365D;
            padding: 9px 10px;
            text-align: left;
            font-size: 14px;
            border: 1px solid #A8BCD9;
        }

        .grid td {
            padding: 8px 10px;
            font-size: 13px;
            border: 1px solid #D5DFED;
            overflow-wrap: anywhere;
            vertical-align: middle;
        }

        .grid tr:nth-child(even) { background: #F4F7FB; }

        .inverter-id {
            font-size: 13px;
            font-weight: bold;
        }

        .inverter-input {
            width: 100%;
            max-width: 250px;
            height: 34px;
            padding: 7px 9px;
            border: 1px solid #A8BCD9;
            border-radius: 6px;
            font-size: 13px;
            color: #17365D;
        }

        .button-row {
            display: flex;
            flex-wrap: wrap;
            gap: 8px;
            margin: 22px 10px 5px;
            padding-top: 14px;
            border-top: 1px solid #A8BCD9;
        }

        .action-button {
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 8px;
            padding: 8px 16px;
            font-size: 14px;
            font-weight: bold;
            cursor: pointer;
        }

        .action-button:hover {
            background: #6E8DBA;
            color: #FFFFFF;
        }

        .message {
            display: block;
            margin: 12px 10px 0;
            font-size: 13px;
            overflow-wrap: anywhere;
        }

        @media (max-width: 600px) {
            body { padding: 8px; }
            .content { padding: 8px 2px; }
            .detail-card { padding: 10px; }
            .detail-title { font-size: 19px; }

            .detail-row {
                grid-template-columns: 1fr;
                gap: 6px;
                margin: 14px 2px;
            }

            .detail-label { text-align: left; }

            .grid { min-width: 350px; }

            .grid th, .grid td {
                padding: 7px;
                font-size: 12px;
            }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="content">
        <div class="detail-card">

            <div class="detail-heading">
                <asp:Button
                    ID="btnBack"
                    runat="server"
                    Text="‹ Back"
                    CssClass="action-button"
                    CausesValidation="false"
                    OnClick="btnBack_Click" />

                <h2 class="detail-title">Unit Generation</h2>
            </div>

            <h3 class="section-title">Generation Details</h3>

            <!-- Editable Plant ID -->
            <div class="detail-row">
                <asp:Label
                    ID="lblPlantID"
                    runat="server"
                    Text="Plant ID:"
                    AssociatedControlID="txtPlantID"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtPlantID"
                    runat="server"
                    CssClass="detail-input"
                    TextMode="Number"
                    min="1"
                    AutoPostBack="true"
                    OnTextChanged="txtPlantID_TextChanged"
                    placeholder="Enter Plant ID" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblGenerationDate"
                    runat="server"
                    Text="Generation Date:"
                    AssociatedControlID="txtGenerationDate"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtGenerationDate"
                    runat="server"
                    TextMode="Date"
                    CssClass="detail-input" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblPGVCL"
                    runat="server"
                    Text="PGVCL:"
                    AssociatedControlID="txtPGVCL"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtPGVCL"
                    runat="server"
                    TextMode="Number"
                    step="any"
                    CssClass="detail-input"
                    placeholder="Enter PGVCL value" />
            </div>

            <div class="detail-row">
                <asp:Label
                    ID="lblGETCO"
                    runat="server"
                    Text="GETCO:"
                    AssociatedControlID="txtGETCO"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtGETCO"
                    runat="server"
                    TextMode="Number"
                    step="any"
                    CssClass="detail-input"
                    placeholder="Enter GETCO value" />
            </div>

            <h3 class="section-title">
                Inverter-wise Generation Values
            </h3>

            <div class="table-wrapper">
                <asp:GridView
                    ID="gvInverterValues"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="grid"
                    GridLines="None"
                    EmptyDataText="No active inverters found for this plant.">

                    <Columns>
                        <asp:TemplateField HeaderText="Inverter ID">
                            <ItemTemplate>
                                <asp:HiddenField
                                    ID="hfInverterID"
                                    runat="server"
                                    Value='<%# Eval("InverterID") %>' />

                                <asp:Label
                                    ID="lblInverterID"
                                    runat="server"
                                    CssClass="inverter-id"
                                    Text='<%# Eval("InverterID") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Generation Value">
                            <ItemTemplate>
                                <asp:TextBox
                                    ID="txtInverterValue"
                                    runat="server"
                                    TextMode="Number"
                                    step="any"
                                    CssClass="inverter-input"
                                    placeholder="Enter value" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <!-- INVTWISE appears after inverter table -->
            <div class="detail-row">
                <asp:Label
                    ID="lblINVTWISE"
                    runat="server"
                    Text="INVTWISE Total:"
                    AssociatedControlID="txtINVTWISE"
                    CssClass="detail-label" />

                <asp:TextBox
                    ID="txtINVTWISE"
                    runat="server"
                    CssClass="detail-input readonly"
                    ReadOnly="true"
                    placeholder="Total generation value" />
            </div>

            <div class="button-row">
                <asp:Button
                    ID="btnSave"
                    runat="server"
                    Text="Save"
                    CssClass="action-button"
                    OnClick="btnSave_Click" />

                <asp:Button
                    ID="btnClear"
                    runat="server"
                    Text="Clear"
                    CssClass="action-button"
                    CausesValidation="false"
                    OnClientClick="clearGenerationForm(); return false;" />
            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message" />

        </div>
    </div>

    <script>
        function calculateInverterTotal() {
            var grid = document.getElementById(
                '<%= gvInverterValues.ClientID %>');

            var totalBox = document.getElementById(
                '<%= txtINVTWISE.ClientID %>');

            if (!grid || !totalBox) return;

            var inputs = grid.querySelectorAll('input[type="number"]');
            var total = 0;

            inputs.forEach(function (input) {
                var value = parseFloat(input.value);

                if (!isNaN(value)) {
                    total += value;
                }
            });

            totalBox.value = total.toFixed(2);
        }

        function clearGenerationForm() {
            document.getElementById(
                '<%= txtGenerationDate.ClientID %>').value = '';

            document.getElementById(
                '<%= txtPGVCL.ClientID %>').value = '';

            document.getElementById(
                '<%= txtGETCO.ClientID %>').value = '';

            document.getElementById(
                '<%= txtINVTWISE.ClientID %>').value = '';

            document.getElementById(
                '<%= lblMessage.ClientID %>').innerText = '';

            var grid = document.getElementById(
                '<%= gvInverterValues.ClientID %>');

            if (grid) {
                grid.querySelectorAll('input[type="number"]')
                    .forEach(function (input) {
                        input.value = '';
                    });
            }
        }

        document.addEventListener('input', function (event) {
            if (event.target &&
                event.target.id &&
                event.target.id.indexOf('txtInverterValue') !== -1) {
                calculateInverterTotal();
            }
        });

        document.addEventListener('DOMContentLoaded', function () {
            calculateInverterTotal();
        });
    </script>

</form>
</body>
</html>
