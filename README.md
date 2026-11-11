<html>
	<head>
<meta name="google-site-verification" content="38kYglhbxjlpieXktL1PYbHf0OWmXSAeWUHgsBjtmn0" />
<meta name="msvalidate.01" content="38F9DFAC225565BA82DE475535B2BC6A" />
</head>
	<body>
	</body>
</html>

# Welcome to ViSo-Nice ERD Application



**A Visual Database Modelling and Management Tool**

<a href="https://github.com/hansievanstraaten/ERD-Application/raw/refs/heads/master/ERD%20Msi%20.Net%2010/ViSo.Viewer.msi" download="ViSo.Viewer.msi">
    <strong>Download ViSo-Nice ERD Application (MSI)</strong>
</a>


## About the Application



ViSo-Nice ERD was developed out of a practical need for a visual representation of database structures, together with the ability to manage database entities directly within that representation.



The application aims to simplify database modelling, development and maintenance by bringing database structures into a visual environment where developers can design, explore and manage their database models.



## Key Features



### Multiple Database Connections



The application supports multiple database connections without requiring separate instances of an ERD model.



Alternative database connections do not require passwords, making it particularly useful in environments where production databases are not directly accessible to developers. This allows developers to maintain and work with database models across different environments without needing direct production database credentials.



### Multiple Canvases per Model



A single ERD model can contain multiple canvases, allowing large database structures to be divided into smaller, more manageable sections while remaining part of one overall model.



Each canvas is saved independently. This makes it possible for multiple developers to work on the same ERD model without creating unnecessary file merge conflicts.



Canvas locking is planned for a future release to prevent multiple developers from modifying the same canvas simultaneously.



### JSON-Based File Storage



ERD models are stored in JSON format, making the underlying files accessible and editable outside the application.



For those who are brave enough to edit the files directly, the structure is there to explore. However, manual modifications may affect the integrity of the model, and correct application functionality cannot be guaranteed after external changes.



### Canvas-Based Database Management



Only tables placed on a canvas are considered active for operations such as:



* Database Reverse Engineering.
* Database Forward Engineering.
* File Scripting.



This provides greater control over which database entities are included in development and deployment operations.



### File Scripting



File scripting currently supports C#, allowing developers to generate scripts from their ERD models.



I would love to extend this functionality to support additional programming languages. If you have a particular language in mind, feel free to get in touch.



Alternatively, you are welcome to download the source code and contribute your own language implementation. I would appreciate it if you shared your tested and working contributions with the project.



Scripting has its complexities, and I intend to provide additional technical documentation as soon as possible.



### Database Support



Current development supports Microsoft SQL Server and Postgres.



Expanding support to additional database platforms is one of the goals for future development. However, database-specific implementation introduces some additional complexities, and contributions or specialist assistance in this area would be greatly appreciated.



### Virtual Relationships



Virtual relationships allow developers to define relationships between tables without creating actual database constraints.



These relationships exist exclusively within the ERD model and are not deployed to the database.



This is particularly useful when working with larger database structures where logical relationships are important for documentation and visual understanding, but enforcing those relationships through database constraints is not necessarily desirable.



Virtual relationships provide the flexibility to document and visualize database relationships without introducing additional database-level constraints or potentially affecting data operations.



## Performance and Scalability



ViSo-Nice ERD is designed to accommodate both small and larger database structures.



During testing, the application was used against a database containing approximately 900 tables. Although certain operations, particularly database comparisons, can become slower with larger models, overall performance has been encouraging.


## Git
Git support provides a manner for teams to share ERD models. Where Locking attempts to prevent developers from editing the same canvas at the same time by having the project in a shared folder, Git, on the other hand, depends on file merging to ensure model integrity. 

Both techniques can be tested, and where File locking is a simple solution, Git provides a more complex but trusted mythology.

## Report Builder
This provides a Visual Report Designer with Integrated Database Reporting and API Support, with separate headers and footers to make it easier on maintenance tasks such as changing company logo's and contact information.

The source code have a working Report API to illustrate how reports can be accessed for integration with your current system.

Note: The reporting solution requires a database and currently supports SQLite, MS SQL and Postgres

## Technology



ViSo-Nice ERD is a Windows desktop application developed using:



* **Programming Language:** C#
* **UI Framework:** Windows Presentation Foundation (WPF)
* **Runtime:** .NET 10



## Development and Updates



Development is ongoing, and updates, improvements and new functionality will be released periodically and without prior notice.



The MSI installation package will be updated with each stable release, so the installer may change as new versions become available.



## Final Thoughts



ViSo-Nice ERD started as a practical solution to a development need, and I hope it can be equally useful to other developers.



There is still plenty of room for improvement, and contributions, suggestions and ideas are always welcome.



Feel free to explore the application, experiment with its functionality and make use of it in your own development environment.



**Happy Hunting!**



*Hope you find it as useful as I have.*


