# DelivAssist

**View Live App:** https://delivassist.netlify.app/

DelivAssist is a Full-Stack web application that empowers gig delivery drivers (DoorDash, UberEats) to track earnings, shifts, and expenses, and gain key performance insights.

## Backend Features Include
- **JWT Authentication** - Secure login and signup using JSON Web Tokens
- **CRUD operations** - Create, read, update, and delete Deliveries, Shifts, and Expenses
- **Shift association** - Deliveries can be associated with specific shifts
- **User profile management** - Users can update their profiles securely
- **Analytics Endpoint** - Backend computes average total, base, and tip pays, dollar-per-mile, most used and highest paying apps, highest paying restaurant and neighborhood, and average monthly expenses both total and by type.
- **Charts and ML** - Python service produces charts for earnings over time, tips by neighborhood, and base pay by app as well as predicts earnings for a particular delivery based on input time, neighborhood, and app.

## Frontend Features Include
- **Responsive Navigation** - Role aware navigation bar that updates on login/logout
- **Delivery Logger** - Easy-to-use forms to add, update, and remove delivery records
- **Shift Tracker** - View, manage, and analyze delivery shifts
- **Expense Tracker** - Log and categorize work-related expenses
- **Dashboard** - Summary of performance statistics including charts and an earnings predictor.
- **Search and Filters** - Quickly find deliveries, shifts, and expenses based on app, type, date, dollar amount, etc.

## Tech Stack
- **Frontend** - TypeScript, React, HTML, CSS
- **Backend** - C#, ASP.NET, SQLite
