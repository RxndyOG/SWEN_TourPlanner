# SWEN_TourPlanner
von Velid Heldic und Stefan Werner | https://github.com/RxndyOG/SWEN_TourPlanner

**Description:** \
Tour Planner is an application based on the GUI frameworks C# WPF. The user creates (bike-, hike-, running- or vacation-) tours in advance and manages the logs and statistical data of accomplished tours.

## Design
### UX (Wireframe)
We divide our application into three sections side-by-side. The left section is the smallest and is responsible for the navigation of the application using dashboard icons. Based on the currently selected icon the content of the other parts changes.
The middle section displays a list of tours the user can choose from. The actual content of the tour and the tour logs are shown in a grid on the right and biggest section. The base stays the same, only the content changes based on the selected tour. 
To create a tour or a tour log for a tour, the user can click the specific button to open a form. After submitting the filled in form, the new tour will be listed with the others and the tourlog will be shown in the tour. 

Mockup             |  Implementation
:-------------------------:|:-------------------------:
![](https://github.com/user-attachments/assets/a356565c-e6b5-4230-bcf7-0338b4df4dfc) | ![](https://github.com/user-attachments/assets/e72db8e2-ad53-42dc-a3c8-ac4849a51294)

## Implementation
### Library decisions
* **O/R Mapper**: Dapper, because of its easy use and high ranking in the market
