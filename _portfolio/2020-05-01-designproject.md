---
title: "Capstone Design Project, Cogswell"
excerpt: "Enable the removal of the central support column<br/><img src='/LY.github.io/images/dpcover2.png' width='60%' height = '60%'>"
date: 2020-05-01
collection: portfolio

---

2020 Spring Made by **Peidong Yan** Collaborated with **M.Haddadin**, **A.Bernola**, **P.Allard**, **E.A.Manneh**.
Achieved the highest grade among the class, and later be invited to WSP Montreal office to present our design methodology.  

[View Full Presentation Slides](https://lorenyan98.github.io/LY.github.io/files/FinalPre.pdf)  

[View Full Design Report](https://lorenyan98.github.io/LY.github.io/files/Finalrep.pdf) 

Introduction
---
The segment of the project from WSP that has been given to us involves the deconstruction of an on-ramp and the re-structuring of a pedestrian bridge linking a commercial building on the north side and a parking garage on the south side. The pedestrian bridge is made of concrete and is supported by a central concrete column. The re-structuring of the bridge involves deconstructing of the central concrete column. The bridge has glass windows that must be preserved. The bridge spans 35.6m and is 10.3m above street level. A functioning water pipe is locating on top of the bridge that must remain operational at all time. Disruption to pedestrian and street traffic must be minimised.  

<img src='/LY.github.io/images/dpcover3.png' width='70%' height = '70%'>

Project Objective
---
Our Group has been given requirements and constraints regarding the execution of the project. These factors shaped the design process of the bridge leading up to the final design. It also affected the construction plan. The following were the key considerations: 

*	Ensure the safety of the structure throughout deconstruction and construction while adhering to the NBCC ULS and SLS design
*	Ensure the availability of construction materials used in the project
*	Minimize deflection of the glass exterior & water pipes located on the bridge roof
*	Minimize interruption of pedestrian and road traffic on & under the bridge
*	Maximize the public use space under the bridge and along Cogswell Street, including the deconstruction of the Cogswell Street on-ramp
*	Minimise the use of construction materials for economic and sustainable benefits

Applicable Design Code, Regulations, References, Software
---

* **Design Code, Regulation**
   * National Building Code of Canada (NBCC 2015)
   * CSA S16 Design Standard (2014) 
   * CSA A23.3-14 Design Standard
* **Reference**
   * Concrete Design Handbook 3rd edition
   * Steel Design Handbook 11th edition
 * **Software**
   * SAP 2000 -- Structural modeling and response analysis
   * SketchUp 2019 -- 3D modeling of the structure and detail connections
   * AutoCAD -- bridge footing, column, bearing cap design
   * Vray 4.0 -- 3D model rendering and visualization
   * Microsoft Excel -- load calculations, connection design calculations

Work Breakdown Structure & Project Timeline
---

<img src='/LY.github.io/images/dpchart.png' width='100%' height = '100%'>

Modelling and Analysis
---

For the Cogswell Redevelopment Program, the main objective of the truss configuration design is to **hold the entire concrete bridge without exceeding the maximum allowable deflection**. Due to the consideration of the constructability that the project consists of multiple construction stages, we developed two designs corresponding to a temporary configuration and a permanent configuration respectively. 

Based on the load condition that calculated by **P.Allard**, I created two structural models with SAP2000 and SketchUp

<img src='/LY.github.io/images/dpdesign1.png' width='90%' height = '90%'>

<img src='/LY.github.io/images/dpdesign2.png' width='90%' height = '90%'>

After defining all load cases and load combinations, the structural analysis indicated that the worst-case scenario is under the load combination of 1.1D+1.5L+0.45W. The cage structure was decomposed into seven groups.In SAP2000, the setting for the Auto-Selection list ranges from  
* W100x19 to W360x990 for the W section, 
* L25x25x3 to L200x200x30 for the WT section, and 
* HSS 32x32x3 to HSS 305x305x13 for the HSS section.  
 
<img src='/LY.github.io/images/dpdesign3.png' width='90%' height = '90%'>

Section group:

<img src='/LY.github.io/images/dpdesign4.png' width='90%' height = '90%'>

The software optimizes the structural behavior and prevents potential failures by selecting suitable truss sizes that can resist the worst-case scenario.  The default design code is based on CSA S16-14. 

<img src='/LY.github.io/images/dpdesign7.png' width='90%' height = '90%'>

Auto-Selection Result:

<img src='/LY.github.io/images/dpdesign8.png' width='90%' height = '90%'>

After defining the geometry and size of trusses, the model passed to SAP2000 for further check 

<img src='/LY.github.io/images/dpdesign5.png' width='90%' height = '90%'>

Detailed Design
---

A detailed connection design follows the SAP 2000 frame analysis. First, the results of the analysis data are exported to a Microsoft Excel file. SAP 2000 exports analysis results in a tabular format. Each row in the table corresponds to a point on an individual frame element and the loads acting at that point. 

<img src='/LY.github.io/images/dpdesign9.png' width='70%' height = '70%'>

Summary of maximum forces in all truss members

<img src='/LY.github.io/images/dpdesign10.png' width='70%' height = '70%'>

Following the decision to design our connections as basic welded & bolted gusset plate connections, conceptual drawings are drafted using Google SketchUp to aid visually in the design steps and provide rough sketches & details of the future connections. An example of such is shown in the picture below. The other conceptual connection models done in SketchUp are found in the annex section of this report. 

As there are 14 different types of connection in our design, five of them are selected for information. More detailed calculation can be obtained from the [full design report](https://lorenyan98.github.io/LY.github.io/files/Finalrep.pdf) 

<img src='/LY.github.io/images/dpdesign11.png' width='90%' height = '90%'>

<img src='/LY.github.io/images/dpdesign12.png' width='90%' height = '90%'>

Column Design
---

The support connection design encompasses the transfer of forces from the truss structure to the outer columns. The chosen type of support system is a roller and pin type because of the zero moment values at each support. A total of 4 supports are needed.

Our design includes the following components in order to properly design the outer columns:

* Elastomeric Bearings
* Base Plates
* Anchor Rods
* Guided Supports

The final design has the following cross section as shown in the figure.

<img src='/LY.github.io/images/dpdesign14.png' width='90%' height = '90%'>

<img src='/LY.github.io/images/dpdesign15.png' width='90%' height = '90%'>

Construction Stages
---
The Link below is the animation for the construction stage.

[![Watch the video](/LY.github.io/images/dpdesign16.png)](https://www.youtube.com/watch?v=rs_Uda9Jjhg&ab_channel=PeidongYan)


Model Rendering (Construction Complete)
---
<img src='/LY.github.io/images/Final1.png' width='90%' height = '90%'>

<img src='/LY.github.io/images/Final2.png' width='90%' height = '90%'>

<img src='/LY.github.io/images/Final3.png' width='90%' height = '90%'>
