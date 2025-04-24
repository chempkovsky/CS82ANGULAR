- CS82ANGULAR is a Rapid application development tool (RAD). 
  - With CS2ANGULAR the developer can model and generate data management web application with
    - **Frontend** based on ANGULAR framework.
      - tested on ANGULAR 15, 16, 17, 18, 19
      - no longer supported ANGULAR 14, 13, 12, 11, 10, 9 (a few years ago the project started with Angular 9)
    - **Backend** based on net.core 6 (last test on TargetFramework = net9.0).
    - For the **Frontend** the developer can choose
      - [Material Design components for Angular](https://material.angular.io)
      - [Kendo UI for Angular](https://www.telerik.com/kendo-angular-ui) **no longer supported**
      - [Angular powered Bootstrap components](https://ng-bootstrap.github.io/#/home)
    - For the **Frontend** the developer can choose
      - Monolithic app. 
      - Micro-frontend app design using [Module Federation](https://www.angulararchitects.io/aktuelles/the-microfrontend-revolution-module-federation-in-webpack-5/)
      
- Please refer to the [CS82ANGULAR wiki](https://github.com/chempkovsky/CS82ANGULAR/wiki) for details.

- **Important Requirement**:
  - it requires @babel/core to be installed in the **separate** folder. Do not use the folder of your project.
  - please use the following commands to meet this requirement:
````
cd c:\
mkdir babelcore
cd babelcore
npm install --save-dev @babel/core
````

