
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="AttendanceReport.aspx.cs"
    Inherits="PlantMaster.AttendanceReport" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Attendance Report</title>
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />

    <style>
        * {
            box-sizing: border-box;
        }

        body {
            margin: 0;
            padding: 15px;
            min-height: 100vh;
            font-family: Arial, Helvetica, sans-serif;
            background-color: #F4F7FB;
            color: #17365D;
        }

        .content {
            padding: 8px 12px;
        }

        .detail-card {
            width: 100%;
            max-width: 650px;
            padding: 15px;
            background: #EEF3FA;
            border: 1px solid #A8BCD9;
            border-radius: 12px;
            box-shadow: 0 6px 15px rgba(30, 55, 90, .10);
        }

        /* Heading */
        .detail-heading {
            display: flex;
            align-items: center;
            justify-content: flex-start;
            flex-wrap: wrap;
            gap: 8px;
            padding-bottom: 12px;
            border-bottom: 1px solid #A8BCD9;
            margin-bottom: 25px;
        }

        .detail-title {
            font-size: 21px;
            margin: 0;
            color: #17365D;
        }

        /* Form Rows */
        .detail-row {
            display: grid;
            grid-template-columns: 110px minmax(0, 280px);
            align-items: center;
            gap: 12px;
            margin: 15px 10px;
        }

        .detail-label {
            text-align: right;
            font-size: 14px;
            color: #17365D;
        }

        .detail-input {
            width: 100%;
            min-width: 0;
            min-height: 38px;
            padding: 8px 10px;
            border: 1px solid #A8BCD9;
            border-radius: 7px;
            font-size: 14px;
            font-family: Arial, Helvetica, sans-serif;
            color: #17365D;
            background: #FFFFFF;
        }

        .detail-input:focus {
            outline: none;
            border: 2px solid #6E8DBA;
        }

        input[type="file"].detail-input {
            height: auto;
            min-height: 38px;
            padding: 5px;
        }

        /* Buttons */
        .action-button {
            background-color: #A8BCD9;
            color: #17365D;
            border: 1px solid #6E8DBA;
            border-radius: 8px;
            padding: 8px 16px;
            min-height: 38px;
            font-size: 14px;
            font-family: Arial, Helvetica, sans-serif;
            font-weight: bold;
            cursor: pointer;
        }

        .action-button:hover {
            background-color: #6E8DBA;
            color: #FFFFFF;
        }

        .button-row {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
            margin: 20px 10px 8px 132px;
        }

        .message {
            display: block;
            margin: 10px;
            font-size: 13px;
            color: #17365D;
            overflow-wrap: anywhere;
        }

        /* Mobile Responsive */
        @media (max-width: 600px) {

            body {
                padding: 8px;
            }

            .content {
                padding: 8px 2px;
            }

            .detail-card {
                padding: 12px;
            }

            .detail-heading {
                margin-bottom: 20px;
            }

            .detail-title {
                font-size: 19px;
            }

            .detail-row {
                grid-template-columns: 1fr;
                gap: 6px;
                margin: 14px 2px;
            }

            .detail-label {
                text-align: left;
            }

            .button-row {
                margin: 18px 2px 8px;
            }

            .action-button {
                flex: 1;
            }
        }
    </style>
</head>

<body>

    <form id="form1" runat="server">

        <div class="content">

            <asp:Panel ID="pnlDetails"
                runat="server"
                CssClass="detail-card">

                <!-- Heading -->
                <div class="detail-heading">

                    <h2 class="detail-title">
                        Attendance Report
                    </h2>

                </div>

                <!-- Attendance Date -->
                <div class="detail-row">

                    <asp:Label ID="lblDate"
                        runat="server"
                        Text="Date:"
                        AssociatedControlID="txtDate"
                        CssClass="detail-label" />

                    <asp:TextBox ID="txtDate"
                        runat="server"
                        TextMode="Date"
                        CssClass="detail-input" />

                </div>

                <!-- Person Name -->
                <div class="detail-row">

                    <asp:Label ID="lblName"
                        runat="server"
                        Text="Name:"
                        AssociatedControlID="txtName"
                        CssClass="detail-label" />

                    <asp:TextBox ID="txtName"
                        runat="server"
                        CssClass="detail-input"
                        MaxLength="100"
                        placeholder="Enter name" />

                </div>

                <!-- Photo Type -->
                <div class="detail-row">

                    <asp:Label ID="lblPhotoType"
                        runat="server"
                        Text="Photo Type:"
                        AssociatedControlID="ddlPhotoType"
                        CssClass="detail-label" />

                    <asp:DropDownList ID="ddlPhotoType"
                        runat="server"
                        CssClass="detail-input"
                        onchange="changePhotoType();">

                        <asp:ListItem Text="In Photo" Value="In" />
                        <asp:ListItem Text="Out Photo" Value="Out" />

                    </asp:DropDownList>

                </div>

                <!-- In Photo -->
                <div class="detail-row" id="inPhotoRow">

                    <asp:Label ID="lblInPhoto"
                        runat="server"
                        Text="In Photo:"
                        AssociatedControlID="fuInPhoto"
                        CssClass="detail-label" />

                    <div>

                        <asp:FileUpload ID="fuInPhoto"
                            runat="server"
                            CssClass="detail-input"
                            accept="image/jpeg,image/png"
                            onchange="previewPhoto(this, 'inPreview');" />

                        <img id="inPreview"
                            alt="In Photo Preview"
                            style="display:none; width:90px; height:90px;
                            object-fit:cover; margin-top:8px;
                            border:1px solid #A8BCD9; border-radius:6px;" />

                    </div>

                </div>

                <!-- In Time -->
                <div class="detail-row" style="display:none;">

                    <asp:Label ID="lblInTime"
                        runat="server"
                        Text="In Time:"
                        AssociatedControlID="txtInTime"
                        CssClass="detail-label" />

                    <asp:TextBox ID="txtInTime"
                        runat="server"
                        TextMode="Time"
                        CssClass="detail-input" />

                </div>

                <!-- Out Photo -->
                <div class="detail-row" id="outPhotoRow"
                    style="display:none;">

                    <asp:Label ID="lblOutPhoto"
                        runat="server"
                        Text="Out Photo:"
                        AssociatedControlID="fuOutPhoto"
                        CssClass="detail-label" />

                    <div>

                        <asp:FileUpload ID="fuOutPhoto"
                            runat="server"
                            CssClass="detail-input"
                            accept="image/jpeg,image/png"
                            onchange="previewPhoto(this, 'outPreview');" />

                        <img id="outPreview"
                            alt="Out Photo Preview"
                            style="display:none; width:90px; height:90px;
                            object-fit:cover; margin-top:8px;
                            border:1px solid #A8BCD9; border-radius:6px;" />

                    </div>

                </div>

                <!-- Out Time -->
                <div class="detail-row" style="display:none;">

                    <asp:Label ID="lblOutTime"
                        runat="server"
                        Text="Out Time:"
                        AssociatedControlID="txtOutTime"
                        CssClass="detail-label" />

                    <asp:TextBox ID="txtOutTime"
                        runat="server"
                        TextMode="Time"
                        CssClass="detail-input" />

                </div>

                <!-- Action Buttons -->
                <div class="button-row">

                    <asp:Button ID="btnSave"
                        runat="server"
                        Text="Save"
                        CssClass="action-button"
                        OnClick="btnSave_Click" />

                    <asp:Button ID="btnClear"
                        runat="server"
                        Text="Clear"
                        CssClass="action-button"
                        CausesValidation="false"
                        OnClick="btnClear_Click" />

                    <asp:Button ID="btnBack"
                        runat="server"
                        Text="Back"
                        CssClass="action-button"
                        CausesValidation="false"
                        OnClick="btnBack_Click" />

                </div>

                <asp:Label ID="lblMessage"
                    runat="server"
                    CssClass="message" />

            </asp:Panel>

        </div>

    </form>

    <!-- JavaScript -->
    <script type="text/javascript">

        function changePhotoType() {

            var dropdown = document.getElementById(
                '<%= ddlPhotoType.ClientID %>');

            var inRow = document.getElementById('inPhotoRow');
            var outRow = document.getElementById('outPhotoRow');

            if (dropdown.value === 'In') {

                inRow.style.display = 'grid';
                outRow.style.display = 'none';

            } else {

                inRow.style.display = 'none';
                outRow.style.display = 'grid';

            }
        }

        function previewPhoto(input, previewId) {

            var preview = document.getElementById(previewId);

            if (input.files && input.files.length > 0) {

                var file = input.files[0];

                if (!file.type.match(/^image\/(jpeg|png)$/i)) {

                    alert('Please select a JPG or PNG image.');

                    input.value = '';

                    preview.removeAttribute('src');
                    preview.style.display = 'none';

                    return;
                }

                var reader = new FileReader();

                reader.onload = function (e) {

                    preview.src = e.target.result;
                    preview.style.display = 'block';

                };

                reader.readAsDataURL(file);

            } else {

                preview.removeAttribute('src');
                preview.style.display = 'none';

            }
        }

        window.onload = function () {

            changePhotoType();

        };

    </script>

</body>
</html>

