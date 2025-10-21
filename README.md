# My Disc Golf Collection

A full-stack web application for managing your disc golf disc collection, built with ASP.NET Core Web API and Svelte.

## Features

✅ **Complete Disc Management**
- Add, edit, and delete discs
- Track all disc details: name, brand, category, flight numbers, plastic, color, weight
- Upload and manage disc photos
- Search and filter your collection

✅ **Four Disc Categories**
- Putter
- Mid
- Fairway  
- Driver

✅ **Modern Web Interface**
- Responsive design that works on desktop and mobile
- Clean, intuitive user interface
- Real-time search and filtering
- Modal forms for adding/editing discs
- Image upload with drag-and-drop

✅ **Local Database**
- SQLite database for local storage
- No internet connection required
- All data stored securely on your machine

## How to Run

### Prerequisites
- .NET 9.0 SDK
- Node.js (v18 or higher)

### Starting the Application

1. **Start the API Backend** (in one terminal):
   ```bash
   dotnet run
   ```
   The API will start on `http://localhost:5000`

2. **Start the Svelte Frontend** (in another terminal):
   ```bash
   cd client
   npm run dev
   ```
   The web app will open on `http://localhost:5173`

3. **Open your browser** and navigate to `http://localhost:5173`

## Project Structure

```
MyDiscgolfDiscs/
├── Controllers/           # Web API controllers
├── Models/               # Data models and database context
├── Services/             # Business logic services
├── wwwroot/images/       # Uploaded disc images
├── client/               # Svelte frontend application
│   ├── src/lib/          # Shared components and API service
│   └── src/routes/       # Svelte pages
├── discs.db             # SQLite database (created on first run)
└── Program.cs           # API startup configuration
```

## API Endpoints

- `GET /api/discs` - Get all discs
- `GET /api/discs/{id}` - Get disc by ID
- `GET /api/discs/category/{category}` - Get discs by category
- `POST /api/discs` - Create new disc
- `PUT /api/discs/{id}` - Update disc
- `DELETE /api/discs/{id}` - Delete disc
- `POST /api/discs/{id}/image` - Upload disc image
- `GET /api/discs/categories` - Get available categories

## Technologies Used

**Backend:**
- ASP.NET Core 9.0 Web API
- Entity Framework Core with SQLite
- Swagger/OpenAPI documentation

**Frontend:**
- SvelteKit with TypeScript
- Vite build tool
- Responsive CSS Grid layout
- File upload handling

## Usage Tips

1. **Adding Your First Disc**: Click the "+ Add Disc" button to open the form
2. **Flight Numbers**: Use standard disc golf notation (Speed|Glide|Turn|Fade)
3. **Images**: Click the camera icon on any disc to upload a photo
4. **Searching**: Use the search box to find discs by name, brand, or description
5. **Filtering**: Use the category dropdown to view only specific disc types

## Database

The SQLite database (`discs.db`) is created automatically on first run. Disc images are stored in the `wwwroot/images/` folder and referenced in the database.

Your data is stored locally and never sent to any external servers.

Enjoy managing your disc golf collection! 🥏
