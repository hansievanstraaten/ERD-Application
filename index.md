<html>

<head>
<meta http-equiv=Content-Type content="text/html; charset=windows-1252">
<meta name=Generator content="Microsoft Word 15 (filtered)">
<style>
<!--
 /* Font Definitions */
 @font-face
	{font-family:"Cambria Math";
	panose-1:2 4 5 3 5 4 6 3 2 4;}
@font-face
	{font-family:Calibri;
	panose-1:2 15 5 2 2 2 4 3 2 4;}
 /* Style Definitions */
 p.MsoNormal, li.MsoNormal, div.MsoNormal
	{margin-top:0cm;
	margin-right:0cm;
	margin-bottom:8.0pt;
	margin-left:0cm;
	line-height:107%;
	font-size:11.0pt;
	font-family:"Calibri",sans-serif;}
.MsoChpDefault
	{font-family:"Calibri",sans-serif;}
.MsoPapDefault
	{margin-bottom:8.0pt;
	line-height:107%;}
@page WordSection1
	{size:595.3pt 841.9pt;
	margin:72.0pt 72.0pt 72.0pt 72.0pt;}
div.WordSection1
	{page:WordSection1;}
-->
</style>

</head>

<body lang=EN-ZA>

<div class=WordSection1>

<p class=MsoNormal><span lang=EN-US>Welcome to ViSo-Nice ERD Application.</span></p>

<p class=MsoNormal><span lang=EN-US>The Application was developed as part of a necessity
to have a visual representation on a database and managing database entities as
part of the representation.</span></p>

<p class=MsoNormal><span lang=EN-US>Support for multiple databases, without the
necessity to create a new instance of the ERD Model. Alternative database
connections are not required to provide a password that makes this application
ideal for deployment to production environments where the production database
is not available for development.</span></p>

<p class=MsoNormal><span lang=EN-US>Multiple canvases per model allows for smaller
canvases and acts as one great ERD. Each canvas is saved on its own that allows
for developers to work on the same ERD Model without Merage conflicts. For
Later development the locking of canvases will be implemented to ensure that developers
do not work on the same canvas at the same time.</span></p>

<p class=MsoNormal><span lang=EN-US>Files are saved in Json format that
provides editability in raw file format, for those who are brave, though I
cannot guarantee correct application functionality one a file has been edited.</span></p>

<p class=MsoNormal><span lang=EN-US>Only table that are on a canvas are active,
and are updated during Reverse and Forward Engineering, as well as File
Scripting.</span></p>

<p class=MsoNormal><span lang=EN-US>File Scripting is available for C#, but I
would love to add more languages, if you would like a language implementation to
feel free to ask or download the source and add the functionality yourself. I
would however like if you share your changes once it is tested and correct.
Scripting is a bit tricky and I will provide a document as soon as posable on
this matter.</span></p>

<p class=MsoNormal><span lang=EN-US>Virtual relations allow for relations to be
created without any database constraint. These types of relations are only
visible on the canvas and will not be deployed to your database. Thus, you can
create relations on your ERD and don’t need to worry about database performance
on larger database models where data IO is of more importance than relational
management.</span></p>

<p class=MsoNormal><span lang=EN-US>&nbsp;</span></p>

<p class=MsoNormal><span lang=EN-US>Current development only supports MS SQL,
and as with the scripting languages, I would like to extend this, though this
will be a bit trickier and some specialist assistance in the database arena would
be appreciated.</span></p>

<p class=MsoNormal><span lang=EN-US>The Application are ideal for small to
larger size databases, and I tested it on a database consisting of about 900
tables. Tends to be a bit slow on some functions such as database comparisons, but
in general performance was good.</span></p>

<p class=MsoNormal><span lang=EN-US>This desktop application is developed in C#
using WPF, .Net Framework 4.7.</span></p>

<p class=MsoNormal><span lang=EN-US>Upgrades and changes will be periodic, and
without notice. </span></p>

<p class=MsoNormal><span lang=EN-US>The Msi Build will change after each stable
version.</span></p>

<p class=MsoNormal><span lang=EN-US>Happy Huntings</span></p>

<p class=MsoNormal><span lang=EN-US>Hope you can use this application to your
own benefit.</span></p>

<p class=MsoNormal><span lang=EN-US>&nbsp;</span></p>

</div>

</body>

</html>
