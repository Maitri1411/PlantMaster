
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AttendancePrint.aspx.cs"
    Inherits="PlantMaster.AttendancePrint" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms"
    Namespace="Microsoft.Reporting.WebForms"
    TagPrefix="rsweb" %>

<!DOCTYPE html>
<html>
<head runat="server">

    <title>Attendance Report</title>

    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 20px;
            font-family: Arial, Helvetica, sans-serif;
            background: #F4F7FB;
            color: #17365D;
        }

        .container {
            width: 100%;
            max-width: 1100px;
            margin: 0 auto;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 10px;
            padding: 20px;
        }

        .header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            gap: 10px;
            margin-bottom: 20px;
            flex-wrap: wrap;
        }

        .header h2 {
            margin: 0;
            font-size: 22px;
            color: #17365D;
        }

        .back-button {
            min-height: 32px;
            padding: 6px 12px;
            background: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 6px;
            font-size: 13px;
            font-weight: bold;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
        }

        .back-button:hover {
            background: #6E8DBA;
            color: white;
        }

        .report-box {
            width: 100%;
            overflow-x: auto;
            background: white;
            border: 1px solid #A8BCD9;
            border-radius: 6px;
            padding: 10px;
        }

        @media screen and (max-width: 600px) {
            body {
                padding: 10px;
            }

            .container {
                padding: 12px;
            }

            .header h2 {
                font-size: 19px;
            }
        }
    </style>

</head>

<body>

    <form id="form1" runat="server">

        <asp:ScriptManager
            ID="ScriptManager1"
            runat="server">
        </asp:ScriptManager>

        <div class="container">

            <div class="header">

                <h2>Attendance Report</h2>

                <asp:HyperLink
                    ID="lnkBack"
                    runat="server"
                    NavigateUrl="~/AttendanceList.aspx"
                    CssClass="back-button">
                    Back
                </asp:HyperLink>

            </div>

            <div class="report-box">

                <rsweb:ReportViewer
                    ID="rvAttendance"
                    runat="server"
                    Width="100%"
                    Height="650px"
                    ProcessingMode="Local"
                    AsyncRendering="false"
                    SizeToReportContent="true">
                </rsweb:ReportViewer>

            </div>

            <asp:Label
                ID="lblMessage"
                runat="server"
                ForeColor="Red"
                Visible="false">
            </asp:Label>

        </div>

    </form>

</body>
</html>
