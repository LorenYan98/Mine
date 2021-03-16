---
title: "DesignCheck2, Arup Compute"
excerpt: "**The project aims to form a systematic check for a W section element**<br/><img src='/LY.github.io/images/designcheck.png' width='50%' height = '50%'> "
date: 2021-03-08
tags:
  - Design Check
  - Analysis  
collection: portfolio
---

2021 Spring Made by **Peidong Yan** Directed by **R.Chana**. 

[Design Check sample output](https://lorenyan98.github.io/LY.github.io/files/DesignCheck2.Structural.CombinedBeamChecks.pdf)

**DesignCheck is a standard framework for writing engineering calculations in code**. The W-section DesignCheck aims to provide the simplest way to compute a systematic check for a W section element in accordance with Canadian Code CSA S16-14, one-click with a full calculation report. 

This project will be contributing to a central store of Arup knowledge that will be useful forever.

* Prgramming language: C#
* Platform: Visual Studio Code, Gitlab  

User input
---
The code requires the user to input section properties and loading conditions along the major and minor axis. Detailed explanation for each variable will be automatically displayed when the cursor points to it.

<img src='/LY.github.io/images/designsc.png'>

DesignCheck Methodology (Partial Flowchat)
---

The code reads through user's input and select proper check based on it.
* _S16-14 11.3, 11.2_, Section Classification 
* _S16-14 13.3_, Compression
* _S16-14 13.2_, Tension
* _S16-14 13.4_, Shear
* _S16-14 13.5, 13.6_, Bending, Biaxial Bending (Calculation varies upon differet section classes)
* _S16-14 13.3, 13.8_, Compression + Bending (Calculation varies upon differet section classes)
    * Cross section check
    * Overall check
    * Lateral torsional buckling check
* _S16-14 13.9_, Tension + Bending (Calculation varies upon differet section classes)

  <img src='/LY.github.io/images/flowchart.png'>

Sample Code
---
The code below shows the methodology for the **section classification**. As there are some existing functions available for me to process this, the code calls the existing function and outputs the section class for the following calculations. 
```C#
            // Section class checks flexural compression
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.SectionClassFlexuralCompressionW section_class_flex_calc;
            section_class_flex_calc = new Steel.CSA.S16_14.Members.MomentResistance.SectionClassFlexuralCompressionW(calculationID, F_y.Value, b.Value, d.Value, t.Value, w_web.Value, C_f.Value, A.Value);
            DeclareSubcalculation(section_class_flex_calc, false);
            if (this.ErrorsExist)
                return;
            DesignCheck2.Definitions.Definition flange_class_ = new DesignCheck2.Definitions.Definition("flange_class", "Flange\\ class", "", "Section class of flange");
            DesignCheck2.Enums.Structural.Steel.CSA.SectionClass flange_class = section_class_flex_calc.Results_Class_Flange.ValueTyped;
            DesignCheck2.Enums.Structural.Steel.CSA.SectionClass web_class = section_class_flex_calc.Results_Class_Web.ValueTyped;
            if (section_class_flex_calc.Results_Class_Flange.ValueTyped == DesignCheck2.Enums.Structural.Steel.CSA.SectionClass.Class_4)
                AddError("Flange is Class 4!");
            if (section_class_flex_calc.Results_Class_Web.ValueTyped == Enums.Structural.Steel.CSA.SectionClass.Class_4)
                AddError("Web is Class 4!");
```
The code below shows the methodology for the **flexural resistance check**. Based on the result of section classification from the previous calc, the code calls the function that is used to calculate the bending moment resistance.
```C#
            Variable M_r_x = DeclareVariableSilently(0, DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_x.Properties);
            if (input_lateral_support == DesignCheck2.Enums.Structural.Steel.CSA.LateralSupportCondition.Laterally_Supported)
            {
                if ((flange_class == Enums.Structural.Steel.CSA.SectionClass.Class_1 || flange_class == Enums.Structural.Steel.CSA.SectionClass.Class_2) && (web_class == Enums.Structural.Steel.CSA.SectionClass.Class_1 || web_class == Enums.Structural.Steel.CSA.SectionClass.Class_2))
                {
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallySupported.MomentResistanceClass1_2 moment_strong_calc;
                    moment_strong_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallySupported.MomentResistanceClass1_2(calculationID, F_y.Value, Z_x.Value);
                    DeclareSubcalculation(moment_strong_calc, false);
                    if (this.ErrorsExist)
                        return;
                    M_r_x = GetSubcalculationResult(moment_strong_calc, moment_strong_calc.Results_M_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_x.Properties);
                }
```  

Output
---
As a result, a full calculation report for the W section element in accordance with Canadian Code CSA S16-14 will be generated. The reports include all detailed calculation and reference from the Building Code, which significantly accelerates the design check process. 

[View Full Calculation Report](https://lorenyan98.github.io/LY.github.io/files/DesignCheck2.Structural.CombinedBeamChecks.pdf) 

  <img src='/LY.github.io/images/samplecal.png'>


