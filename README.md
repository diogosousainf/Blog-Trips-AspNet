<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Travel Blog Project README</title>
</head>
<body>
    <h1>Travel Blog Project</h1>
    <p>Welcome to the Travel Blog Project! This is an ASP.NET web application where users can share their travel experiences, post images, and add tags to their posts.</p>
   <img src="1.png" alt="Screenshot 1">

    <h2>Features</h2>
    <ul>
        <li><strong>User Registration and Authentication</strong>: Users can sign up and log in to the application.</li>
        <li><strong>Create, Read, Update, Delete (CRUD) Operations</strong>: Users can create, view, edit, and delete their travel posts.</li>
        <li><strong>Image Upload</strong>: Users can upload images to their travel posts.</li>
        <li><strong>Tagging</strong>: Users can add tags to their posts to categorize their travel experiences.</li>
    </ul>

    <h2>Technologies Used</h2>
    <ul>
        <li><strong>ASP.NET Core</strong>: Backend framework for building web applications.</li>
        <li><strong>Entity Framework Core</strong>: ORM for database operations.</li>
        <li><strong>SQL Server</strong>: Database for storing user data and posts.</li>
        <li><strong>Bootstrap</strong>: Frontend framework for responsive design.</li>
        <li><strong>jQuery</strong>: JavaScript library for DOM manipulation and AJAX requests.</li>
    </ul>

    <h2>Getting Started</h2>

    <h3>Prerequisites</h3>
    <ul>
        <li><a href="https://dotnet.microsoft.com/download/dotnet/6.0" target="_blank">.NET 6.0 SDK</a></li>
        <li><a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank">SQL Server</a></li>
    </ul>

    <h3>Installation</h3>
    <ol>
        <li><strong>Clone the repository</strong>
            <pre><code>git clone https://github.com/yourusername/travel-blog.git
cd travel-blog</code></pre>
        </li>
        <li><strong>Set up the database</strong>
            <ul>
                <li>Update the connection string in <code>appsettings.json</code> to point to your SQL Server instance.</li>
                <li>Apply migrations to create the database schema.
                    <pre><code>dotnet ef database update</code></pre>
                </li>
            </ul>
        </li>
        <li><strong>Run the application</strong>
            <pre><code>dotnet run</code></pre>
        </li>
        <li><strong>Access the application</strong>
            <p>Open your web browser and navigate to <a href="https://localhost:5001" target="_blank">https://localhost:5001</a>.</p>
        </li>
    </ol>

    <h2>Usage</h2>
    <h3>Registration and Login</h3>
    <ol>
        <li>Register a new account or log in with your existing credentials.</li>
        <li>Upon successful login, you can start creating and managing your travel posts.</li>
    </ol>
</body>
</html>
