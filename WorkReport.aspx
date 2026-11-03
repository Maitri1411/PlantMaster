
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="WorkReport.aspx.cs"
    Inherits="PlantMaster.WorkReport" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Work Report Details</title>

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 25px;
            font-family: Arial, Helvetica, sans-serif;
            background: #E6E8F2;
            color: #17365D;
        }

        .container {
            width: 100%;
            max-width: 850px;
            margin: 0 auto;
        }

        .card {
            width: 100%;
            padding: 20px;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 12px;
            box-shadow: 0 6px 15px rgba(30,55,90,.10);
        }

        .heading-row {
            display: flex;
            align-items: center;
            gap: 12px;
            padding-bottom: 12px;
            border-bottom: 1px solid #A8BCD9;
            margin-bottom: 18px;
        }

        .page-title {
            margin: 0;
            font-size: 21px;
            font-weight: 600;
        }

        .section-title {
            margin: 0 0 18px;
            padding-bottom: 10px;
            border-bottom: 1px solid #A8BCD9;
            font-size: 17px;
        }

        .form-row {
            display: grid;
            grid-template-columns: 130px minmax(0, 1fr);
            gap: 12px;
            align-items: start;
            margin: 16px 0;
        }

        .form-label {
            padding-top: 10px;
            text-align: right;
            font-size: 14px;
            font-weight: 600;
        }

        .form-input,
        .form-textarea {
            width: 100%;
            padding: 10px 12px;
            background: #FFFFFF;
            color: #17365D;
            border: 1px solid #A8BCD9;
            border-radius: 6px;
            font-family: Arial, Helvetica, sans-serif;
            font-size: 14px;
        }

        .form-input {
            height: 40px;
        }

        .form-textarea {
            min-height: 110px;
            resize: vertical;
        }

        .button-row {
            display: flex;
            gap: 10px;
            margin-top: 22px;
            padding-top: 15px;
            border-top: 1px solid #A8BCD9;
        }

        .action-button {
            min-height: 38px;
            padding: 9px 20px;
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 600;
            cursor: pointer;
        }

        .action-button:hover {
            background: #6E8DBA;
            color: #FFFFFF;
        }

        .message {
            display: block;
            margin-top: 15px;
            font-size: 14px;
            overflow-wrap: anywhere;
        }

        /* MOBILE DESIGN */
        @media (max-width: 600px) {

            body {
                padding: 12px;
            }

            .container {
                width: 100%;
                max-width: 100%;
            }

            .card {
                width: 100%;
                padding: 14px;
                border-radius: 10px;
            }

            .heading-row {
                gap: 8px;
                margin-bottom: 14px;
                padding-bottom: 10px;
            }

            .page-title {
                font-size: 19px;
            }

            .section-title {
                font-size: 15px;
                margin-bottom: 15px;
            }

            .form-row {
                display: flex;
                flex-direction: column;
                gap: 7px;
                margin: 14px 0;
            }

            .form-label {
                text-align: left;
                padding-top: 0;
                font-size: 13px;
            }

            .form-input {
                width: 100%;
                height: 36px;
                font-size: 13px;
            }

            .form-textarea {
                width: 100%;
                min-height: 95px;
                font-size: 13px;
            }

            .button-row {
                display: flex;
                gap: 8px;
                margin-top: 18px;
                padding-top: 13px;
            }

            .button-row .action-button {
                flex: 1;
                width: 50%;
                min-width: 0;
                padding: 9px 5px;
                font-size: 13px;
            }

            .heading-row .action-button {
                padding: 8px 12px;
                min-height: 34px;
                font-size: 12px;
            }

            .message {
                font-size: 13px;
            }
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="container">
        <div class="card">

            <div class="heading-row">

                <asp:Button
                    ID="btnBack"
                    runat="server"
                    Text="Back"
                    CssClass="action-button"
                    CausesValidation="false"
                    OnClick="btnBack_Click" />

                <h2 class="page-title">
                    <asp:Label
                        ID="lblHeading"
                        runat="server"
                        Text="Work Report" />
                </h2>

            </div>

            <h3 class="section-title">Work Report Details</h3>

            <div class="form-row">

                <asp:Label
                    ID="lblDate"
                    runat="server"
                    Text="Report Date:"
                    AssociatedControlID="txtDate"
                    CssClass="form-label" />

                <asp:TextBox
                    ID="txtDate"
                    runat="server"
                    TextMode="Date"
                    CssClass="form-input" />

            </div>

            <div class="form-row">

                <asp:Label
                    ID="lblWorkDetails"
                    runat="server"
                    Text="Work Details:"
                    AssociatedControlID="txtWorkDetails"
                    CssClass="form-label" />

                <asp:TextBox
                    ID="txtWorkDetails"
                    runat="server"
                    TextMode="MultiLine"
                    Rows="4"
                    MaxLength="250"
                    CssClass="form-textarea"
                    placeholder="Enter work details..." />

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
                    OnClick="btnClear_Click" />

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                CssClass="message" />

        </div>
    </div>

</form>
</body>
</html>
