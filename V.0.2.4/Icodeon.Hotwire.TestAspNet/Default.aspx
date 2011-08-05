<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="Icodeon.Hotwire.TestAspNet._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
<style>
.group { padding:20px; border:1px dashed gray; margin-bottom:20px; padding-top:0px; margin-top:20px; }
h3 { padding-bottom:8px; font-weight:bold; }
</style>
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Hotwire Acceptance Test website
    </h2>
    
    <p>Sample website with Hotwire HttpModule configured to accept restful enqueue requests. (PUT/POST) at the following Uris</p>
    <ul>
    <li>/hotwire/queues.xml</li>
    <li>/hotwire/queues.xml</li>
    <li>/hotwire/queues.json</li>
    </ul>
    
    <p>Enqueued files will be queued for downloading in the /App_Data/HotwireFolders/DownloadQueue folder. A number of "folderwatcher.exe" clients can be fired up that will download and process the queued files, using configured implementations of IFileProcessor.</p>


    <p></p>

</asp:Content>
