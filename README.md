# Microservice CI/CD

A reusable workflow to be used to carry out the CI/CD process of a microservice.
This workflow performs the following steps:

* Checkout the repository with full depth, computing its semantic version
* Build the entire solution
* Run unit tests
* Generate a SQL script with EF Core migrations
* Upload all the build outputs and the SQL script in a GitHub artifact