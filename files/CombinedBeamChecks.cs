using System;
using System.Collections.Generic;
using Arup.Compute.DotNetSdk.Enums;
using DesignCheck2.Framework;
using DesignCheck2.Framework.Attributes;
using DesignCheck2.Helpers;
using static DesignCheck2.Framework.Maths;
using DesignCheck2.Enums.Structural.Steel.CSA;
using DesignCheck2.Structural.Steel.CSA.S16_14.Members.CompressionResistance;
using DesignCheck2.Structural.Steel.CSA.S16_14.Members.TensionResistance;
using DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance;

namespace DesignCheck2.Structural
{
    [Calculation("CSA S16-14 W-Section Element Design","Process a comprehensive design check for a W-section Element in accordance with Canadian Code CSA S16-14.","Loren.Yan@arup.com",LevelOfReview.WorkInProgress)]
    [UUID("819eaeed-177c-418d-93d9-1de748c38ea1")]
    [Funding("076001-90.0")]
    [Revision(0, 0, "Initial revision")]
    [Assumption("Calc is applicable for Class 1, 2 and 3 W sections only")]
    [Assumption("Tension resistance is checked based on yielding only")]
    [Assumption("Compression force is taken as a positive value")]
    [Assumption("Calculation does not check torsion")]
    [Assumption("Calculation for symmetric sections only")]
    public class CombinedBeamChecks : Calculation
    {
        [Result(DesignCheck2.Definitions.Structural.Steel.CSA.Members.C_r.Json)]
        public Result<double> Results_C_r;
        
        [Result(DesignCheck2.Definitions.Structural.Steel.CSA.Members.V_r_x.Json)]
        public Result<double> Results_V_r_x;
        
        [Result(DesignCheck2.Definitions.Structural.Steel.CSA.Members.V_r_y.Json)]
        public Result<double> Results_V_r_y;
        
        [Result(DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_x.Json)]
        public Result<double> Results_M_r_x;
        
        [Result(DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_y.Json)]
        public Result<double> Results_M_r_y;
        
        [Result(DesignCheck2.Definitions.Structural.Steel.CSA.Members.T_r.Json)]
        public Result<double> Results_T_r;

    
        public CombinedBeamChecks(
            [Input(DesignCheck2.Definitions.Framework.ID.Json)]
            string calculationID,
            [Input(DesignCheck2.Definitions.Structural.Analysis.Axi_f.Json)]
             double input_Axi_f,
            [Input(DesignCheck2.Definitions.Structural.Analysis.M_f_x.Json)]
            double input_M_f_x,
            [Input(DesignCheck2.Definitions.Structural.Analysis.M_f_y.Json)]
            double input_M_f_y,
            [Input(DesignCheck2.Definitions.Structural.Analysis.V_f_x.Json)]
            double input_V_f_x,
            [Input(DesignCheck2.Definitions.Structural.Analysis.V_f_y.Json)]
            double input_V_f_y,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.d.Json)]
            double input_d,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.b.Json)]
            double input_b,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.w_web.Json)]
            double input_w_web,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.t.Json)]
            double input_t,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.F_y.Json)]
            double input_F_y,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.F_u.Json)]
            double input_F_u,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.K_x.Json)]
            double input_K_x,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.L_x.Json)]
            double input_L_x,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.K_y.Json)]
            double input_K_y,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.L_y.Json)]
            double input_L_y,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.L_z.Json)]
            double input_L_z,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.n_category.Json)]
            DesignCheck2.Enums.Structural.Steel.CSA.n_SectionType input_n_category,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.x_0.Json)]
            double input_x_0,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.y_0.Json)]
            double input_y_0,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.lateral_support.Json)]
            DesignCheck2.Enums.Structural.Steel.CSA.LateralSupportCondition input_lateral_support,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.omega_2.Json)]
            double input_omega_2,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.omega_1_x.Json)]
            double input_omega_1_x,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.omega_1_y.Json)]
            double input_omega_1_y,
            [Input(DesignCheck2.Definitions.Structural.Steel.CSA.Members.frame_type.Json)]
            DesignCheck2.Enums.Structural.Steel.CSA.FrameType input_frame_type

        )
        {
            AddTitle(calculationID);

            Func<string> staticImageGrabber = () => DesignCheck2.Helpers.RetrieveResource.RetrieveSvg("DesignCheck2.Resources.Images.Structural.Steel.CSA","WBeamReferenceAxis");
            AddSvgImage(staticImageGrabber, "W-Beam Axis", ReportSvgGraphic.ImageSize.Small);

            AddSubHeader("Inputs",false);
            
            Variable C_f,T_f;
            if(input_Axi_f>=0)
            {
               C_f = DeclareVariable(input_Axi_f, DesignCheck2.Definitions.Structural.Analysis.C_f.Properties);
               T_f = DeclareVariable(0, DesignCheck2.Definitions.Structural.Analysis.T_f.Properties);
            }
            else
            {  
               T_f = DeclareVariable(-input_Axi_f, DesignCheck2.Definitions.Structural.Analysis.T_f.Properties);
               C_f = DeclareVariable(0, DesignCheck2.Definitions.Structural.Analysis.C_f.Properties);
            }
            Variable M_f_x = DeclareInput(input_M_f_x, DesignCheck2.Definitions.Structural.Analysis.M_f_x.Properties); // (kN) Bending moment on member about x-axis
            Variable M_f_y = DeclareInput(input_M_f_y, DesignCheck2.Definitions.Structural.Analysis.M_f_y.Properties); // (kN) Bending moment on member about y-axis
            Variable V_f_x = DeclareInput(input_M_f_x, DesignCheck2.Definitions.Structural.Analysis.V_f_x.Properties); // (kN) Shear force in x-axis direction
            Variable V_f_y = DeclareInput(input_M_f_y, DesignCheck2.Definitions.Structural.Analysis.V_f_y.Properties); // (kN) Shear force in y-axis direction
            Variable d = DeclareInput(input_d, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.d.Properties); // (mm) Overall depth of section
            Variable b = DeclareInput(input_b, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.b.Properties); // (mm) Overall flange width
            Variable w_web = DeclareInput(input_w_web, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.w_web.Properties); // (mm) Web thickness
            Variable t = DeclareInput(input_t, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.t.Properties); // (mm) Flange thickness
            Variable F_y = DeclareInput(input_F_y, DesignCheck2.Definitions.Structural.Steel.CSA.F_y.Properties); // (MPa) Yield strength
            Variable F_u = DeclareInput(input_F_u, DesignCheck2.Definitions.Structural.Steel.CSA.F_u.Properties); // (MPa) Tensile strength
            Variable K_x = DeclareInput(input_K_x, DesignCheck2.Definitions.Structural.Steel.CSA.Members.K_x.Properties); // Effective length factor for buckling about x-axis
            Variable L_x = DeclareInput(input_L_x, DesignCheck2.Definitions.Structural.Steel.CSA.Members.L_x.Properties); // (mm) Unbraced length or span of member about principal x-axis direction
            Variable K_y = DeclareInput(input_K_y, DesignCheck2.Definitions.Structural.Steel.CSA.Members.K_y.Properties); // Effective length factor for buckling about y-axis
            Variable L_y = DeclareInput(input_L_y, DesignCheck2.Definitions.Structural.Steel.CSA.Members.L_y.Properties); // (mm) Unbraced length or span of member about principal y-axis direction
            Variable L_z = DeclareInput(input_L_z, DesignCheck2.Definitions.Structural.Steel.CSA.Members.L_z.Properties); // (mm) Unbraced length or span of member about principal z-axis direction
            double n_value = (input_n_category == DesignCheck2.Enums.Structural.Steel.CSA.n_SectionType.HotRolled_HSSClassC) ? 1.34 : 2.24;
            Variable x_0 = DeclareInput(input_x_0, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.x_0.Properties); // (mm) Distance from centroid of shear center in principal x-axis direction
            Variable y_0 = DeclareInput(input_y_0, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.y_0.Properties); // (mm) Distance from centroid of shear center in principal y-axis direction
            DeclareInput_NotNumber(input_lateral_support.ToString(), DesignCheck2.Definitions.Structural.Steel.CSA.Members.lateral_support.Properties); // Whether a member is laterally unsupported or laterally supported
            Variable omega_2 = DeclareInput(input_omega_2, DesignCheck2.Definitions.Structural.Steel.CSA.omega_2.Properties); // Moment resistance coefficient accounting for moment gradient
            Variable omega_1_x = DeclareInput(input_omega_1_x, DesignCheck2.Definitions.Structural.Steel.CSA.omega_1_x.Properties); // Coefficient to determine equivalent uniform bending effect in beam-columns for the principal x-axis
            Variable omega_1_y = DeclareInput(input_omega_1_y, DesignCheck2.Definitions.Structural.Steel.CSA.omega_1_y.Properties); // Coefficient to determine equivalent uniform bending effect in beam-columns for the principal y-axis
            DeclareInput_NotNumber(input_frame_type.ToString(), DesignCheck2.Definitions.Structural.Steel.CSA.Members.frame_type.Properties); // Lateral support condition for a member





            AddSubHeader("Calculation for section properties", false);
            Variable E = DeclareInput(200000, DesignCheck2.Definitions.Structural.Steel.CSA.E.Properties); // (MPa) Modulus of elasticity of steel
            Variable G = DeclareInput(77000, DesignCheck2.Definitions.Structural.Steel.CSA.G.Properties); // (MPa) Shear modulus of steel
            Variable A = CalcVariable(2*b*t + w_web*(d-2*t), DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.A.Properties);
            Variable I_x = CalcVariable(Pow((d-2*t),3)*w_web/12+2*(Pow(t,3)*b/12+t*b*Pow(d-t,2)/4.0), DesignCheck2.Definitions.Structural.Steel.CSA.Members.I_x.Properties);
            Variable I_y = CalcVariable((d-2*t)*w_web/12.0+2*(Pow(b,3)*t/12.0), DesignCheck2.Definitions.Structural.Steel.CSA.Members.I_y.Properties);
            Variable r_x = CalcVariable(Pow((I_x/A),0.5), DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.r_x.Properties);
            Variable r_y = CalcVariable(Pow((I_y/A),0.5), DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.r_y.Properties);
            Variable S_x = CalcVariable(2*I_x/d, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.S_x.Properties);
            Variable S_y = CalcVariable(2*I_y/b, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.S_y.Properties);
            Variable Z_x = CalcVariable(b*t*(d-t)+w_web*Pow(d-2*t,2)/4, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.Z_x.Properties);
            Variable Z_y = CalcVariable(t*Pow(b,2)/2+(d-2*t)*Pow(w_web,2)/4, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.Z_y.Properties);
            Variable J = CalcVariable(((2*b*Pow(t,3)+(d-2*t)*Pow(w_web,3))/3), DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.J.Properties);
            Variable C_w = CalcVariable(I_y*Pow(d-t,2)/4, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.C_w.Properties);

            // Section calss checks aixal compression
            if(C_f>0)
            {
            AddSubHeader("Classification of W section for axial compression", true, ReportSubHeading.SubheadingLevel.Level1);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.CompressionResistance.SectionClassAxialCompressionW section_class_axial_calc;
            section_class_axial_calc = new Steel.CSA.S16_14.Members.CompressionResistance.SectionClassAxialCompressionW(calculationID, F_y.Value, b.Value, d.Value, t.Value, w_web.Value);
            DeclareSubcalculation(section_class_axial_calc, false);
            if (this.ErrorsExist)
                return;
            if (section_class_axial_calc.Results_Flange_Class4.ValueTyped)
                AddError("Flange is Class 4!");
            if (section_class_axial_calc.Results_Web_Class4.ValueTyped)
                AddError("Web is Class 4!");
            }
            
            // Section class checks flexural compression
            AddSubHeader("Classification of W section for flexural compression", true, ReportSubHeading.SubheadingLevel.Level1);
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
 
            //Shear
            AddSubHeader("Shear capacity", true, ReportSubHeading.SubheadingLevel.Level1);
            AddSubHeader("Shear in the y-axis direction, major axis", true, ReportSubHeading.SubheadingLevel.Level3);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.ShearResistance.Unstiffened.ShearResistanceW shear_weak_calc;
            Variable h = DeclareVariableSilently(d.Value - 2 * t.Value, DesignCheck2.Definitions.Structural.Steel.CSA.SectionProperties.h.Properties);
            shear_weak_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.ShearResistance.Unstiffened.ShearResistanceW(calculationID, F_y.Value, d.Value, h.Value, w_web.Value);
            DeclareSubcalculation(shear_weak_calc, false);
            if (this.ErrorsExist)
                return;
            Variable V_r_y = GetSubcalculationResult(shear_weak_calc, shear_weak_calc.Results_V_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.V_r_y.Properties);
            Results_V_r_y = AddResult(V_r_y); // (kN) Factored shear resistance in y-axis direction



            AddSubHeader("Shear in the x-axis direction, minor axis", true, ReportSubHeading.SubheadingLevel.Level3);
            DesignCheck2.Definitions.Definition d__ = new DesignCheck2.Definitions.Definition("d", "d", "mm", "Overall depth of section for weak axis shear (depth of 2 flanges)");
            Variable d_ = CalcVariable(2 * b, d__);
            DesignCheck2.Definitions.Definition h__ = new DesignCheck2.Definitions.Definition("h", "h", "mm", "Clear depth of web between flanges for weak axis shear (width of unstiffened compression elements)");
            Variable h_ = DeclareVariableSilently(b.Value/2,h__);
            DesignCheck2.Definitions.Definition w_web__ = new DesignCheck2.Definitions.Definition("w", "w", "mm", "Web thickness for weak axis shear (flange thickness) ");
            Variable w_web_ = DeclareVariable(t.Value, w_web__);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.ShearResistance.Unstiffened.ShearResistanceW shear_strong_calc;
            shear_strong_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.ShearResistance.Unstiffened.ShearResistanceW(calculationID, F_y.Value, d_.Value, h_.Value, w_web_.Value);
            DeclareSubcalculation(shear_strong_calc, false);
            if (this.ErrorsExist)
                return;
            Variable V_r_x = GetSubcalculationResult(shear_strong_calc, shear_strong_calc.Results_V_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.V_r_x.Properties);
            Results_V_r_x = AddResult(V_r_x); // (kN) Factored shear resistance in x-axis direction


            //Slenderness ratio check
            if(C_f>0)
            {
            AddSubHeader("Slenderness ratio check", true, ReportSubHeading.SubheadingLevel.Level1);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.CompressionResistance.SlendernessRatio SR_calc;
            SR_calc = new Steel.CSA.S16_14.Members.CompressionResistance.SlendernessRatio(calculationID, K_x.Value, K_y.Value, L_x.Value, L_y.Value, r_x.Value, r_y.Value);
            DeclareSubcalculation(SR_calc, false);
            if (this.ErrorsExist)
                return;
            Variable slenderness_ratio = GetSubcalculationResult(SR_calc, SR_calc.Results_slenderness_ratio);
            if (section_class_flex_calc.Results_Class_Flange.ValueTyped == DesignCheck2.Enums.Structural.Steel.CSA.SectionClass.Class_4)
                AddError("Slenderness ratio exceeds maximum permissible slenderness ratio!");
            }

            //Axial compression
            AddSubHeader("Axial compression capacity", true, ReportSubHeading.SubheadingLevel.Level1);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.CompressionResistance.CompressionResistanceDoublySymmetricShapes comp_calc;
            comp_calc = new Steel.CSA.S16_14.Members.CompressionResistance.CompressionResistanceDoublySymmetricShapes(calculationID, A.Value, F_y.Value, E.Value, DesignCheck2.Enums.Structural.Steel.CSA.n_SectionType.HotRolled_HSSClassC, K_x.Value, L_x.Value, r_x.Value, K_y.Value, L_y.Value, r_y.Value, L_z.Value, C_w.Value, G.Value, J.Value, x_0.Value, y_0.Value);
            DeclareSubcalculation(comp_calc, false);
            if (this.ErrorsExist)
                return;
            Variable C_r = GetSubcalculationResult(comp_calc, comp_calc.Results_C_r);
            Results_C_r = AddResult(C_r); // (kN) Factored compressive resistance of a member or component


            // Flexture
            AddSubHeader($"Moment capacity ({input_lateral_support.ToString().ToLower().Replace("_", " ")} member)", true, ReportSubHeading.SubheadingLevel.Level1);
            AddSubHeader("Strong axis bending", true, ReportSubHeading.SubheadingLevel.Level3);

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
                else if ((flange_class == Enums.Structural.Steel.CSA.SectionClass.Class_3) || (web_class == Enums.Structural.Steel.CSA.SectionClass.Class_3))
                {
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallySupported.MomentResistanceClass3 moment_strong_calc;
                    moment_strong_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallySupported.MomentResistanceClass3(calculationID, F_y.Value, S_x.Value);
                    DeclareSubcalculation(moment_strong_calc, false);
                    if (this.ErrorsExist)
                        return;
                    M_r_x = GetSubcalculationResult(moment_strong_calc, moment_strong_calc.Results_M_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_x.Properties);
                }
                else
                {
                    AddWarning("Calculation for Class 4 is not yet implemented - sorry!");
                }
            }
            else
            {
                if ((flange_class == Enums.Structural.Steel.CSA.SectionClass.Class_1 || flange_class == Enums.Structural.Steel.CSA.SectionClass.Class_2) && (web_class == Enums.Structural.Steel.CSA.SectionClass.Class_1 || web_class == Enums.Structural.Steel.CSA.SectionClass.Class_2))
                {
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallyUnsupported.MomentResistanceClass1_2 moment_strong_calc;
                    moment_strong_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallyUnsupported.MomentResistanceClass1_2(calculationID, I_y.Value, J.Value, E.Value, G.Value, omega_2.Value, L_x.Value, C_w.Value, Z_x.Value, F_y.Value);
                    DeclareSubcalculation(moment_strong_calc, false);
                    if (this.ErrorsExist)
                        return;
                    M_r_x = GetSubcalculationResult(moment_strong_calc, moment_strong_calc.Results_M_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_x.Properties);
                }
                else
                {
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallyUnsupported.MomentResistanceClass3_4 moment_strong_calc;
                    moment_strong_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallyUnsupported.MomentResistanceClass3_4(calculationID, I_y.Value, J.Value, E.Value, G.Value, omega_2.Value, L_x.Value, C_w.Value, S_x.Value, F_y.Value);
                    DeclareSubcalculation(moment_strong_calc, false);
                    if (this.ErrorsExist)
                        return;
                    M_r_x = GetSubcalculationResult(moment_strong_calc, moment_strong_calc.Results_M_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_x.Properties);
                }
            }
            Results_M_r_x = AddResult(M_r_x); // (kN) Factored moment resistance about x-axis

            AddSubHeader("Weak axis bending", true, ReportSubHeading.SubheadingLevel.Level3);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallySupported.MomentResistanceClass1_2 moment_weak_calc;
            moment_weak_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.MomentResistance.LaterallySupported.MomentResistanceClass1_2(calculationID, F_y.Value, Z_y.Value);
            DeclareSubcalculation(moment_weak_calc, false);
            if (this.ErrorsExist)
                return;
            Variable M_r_y = GetSubcalculationResult(moment_weak_calc, moment_weak_calc.Results_M_r, DesignCheck2.Definitions.Structural.Steel.CSA.Members.M_r_y.Properties);
            Results_M_r_y = AddResult(M_r_y); // (kN) Factored moment resistance about y-axis



            // Tension
            AddSubHeader("Axial tension capacity", true, ReportSubHeading.SubheadingLevel.Level1);
            DesignCheck2.Structural.Steel.CSA.S16_14.Members.TensionResistance.Yielding tens_calc;
            tens_calc = new Steel.CSA.S16_14.Members.TensionResistance.Yielding(calculationID, A.Value, F_y.Value);
            DeclareSubcalculation(tens_calc, false);
            if (this.ErrorsExist)
                return;
            Variable T_r = GetSubcalculationResult(tens_calc, tens_calc.Results_T_r);
            Results_T_r = AddResult(T_r); // (kN) Factored tensile resistance of a member or component
            
            //Biaxial bending
            Double biaxial_util=0.0;
            AddSubHeader("Biaxial bending", true, ReportSubHeading.SubheadingLevel.Level1);
                DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.MemberUtilisationBiaxialBending biaxial_bending_calc;
                biaxial_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.MemberUtilisationBiaxialBending(calculationID, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value);
                DeclareSubcalculation(biaxial_bending_calc, false);
                if (this.ErrorsExist)
                    return;
                biaxial_util = biaxial_bending_calc.Results_Util.ValueTyped;
            AddResult(GetSubcalculationResult(biaxial_bending_calc, biaxial_bending_calc.Results_Util));
            

            //Combined compression and bending
            Double cross_section_util= 0.0,tens_bending_util= 0.0,overall_util= 0.0,ltb_util= 0.0;
            if((C_f > 0) && (M_f_x > 0||M_f_y > 0))
            {
            AddSubHeader("Combined axial compression and bending Calculation", true, ReportSubHeading.SubheadingLevel.Level1);
                if ((flange_class == SectionClass.Class_1 || flange_class == SectionClass.Class_2) && (web_class == SectionClass.Class_1 || web_class == SectionClass.Class_2))
                {
                    AddSubHeader("Compression and bending cross sectional strength I shaped class1 & 2", true, ReportSubHeading.SubheadingLevel.Level1);
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingCrossSectionalStrengthIShapedClass1_2 combined_comp_bending_calc;
                    combined_comp_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingCrossSectionalStrengthIShapedClass1_2(calculationID, C_f.Value, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value, F_y.Value, E.Value, A.Value, omega_1_x.Value, omega_1_y.Value, L_x.Value, L_y.Value, r_x.Value, r_y.Value, I_x.Value, I_y.Value);
                    DeclareSubcalculation(combined_comp_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    cross_section_util = combined_comp_bending_calc.Results_Util.ValueTyped;

                    AddSubHeader("Compression and bending overall member strength I shaped class1 & 2", true, ReportSubHeading.SubheadingLevel.Level1);
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingOverallMemberStrengthIShapedClass1_2 overall_comp_bending_calc;
                    overall_comp_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingOverallMemberStrengthIShapedClass1_2(calculationID, C_f.Value, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value, F_y.Value, E.Value, A.Value,input_n_category,omega_1_x.Value, omega_1_y.Value, L_x.Value, L_y.Value, r_x.Value, r_y.Value, I_x.Value, I_y.Value,input_frame_type);
                    DeclareSubcalculation(overall_comp_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    overall_util = overall_comp_bending_calc.Results_Util.ValueTyped;
                    
                    AddSubHeader("Compression and bending lateral torsional buckling strength I shaped class1 & 2", true, ReportSubHeading.SubheadingLevel.Level1);
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingLTBStrengthIShapedClass1_2 ltb_comp_bending_calc;
                    ltb_comp_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingLTBStrengthIShapedClass1_2(calculationID, C_f.Value, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value, F_y.Value, E.Value, A.Value, input_n_category, omega_1_x.Value, omega_1_y.Value, K_x.Value, K_y.Value, L_x.Value, L_y.Value,L_z.Value ,r_x.Value, r_y.Value, I_x.Value, I_y.Value, C_w.Value, G.Value, J.Value, x_0.Value, y_0.Value, input_frame_type);
                    DeclareSubcalculation(ltb_comp_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    ltb_util = ltb_comp_bending_calc.Results_Util.ValueTyped;                    


                    AddResult(GetSubcalculationResult(combined_comp_bending_calc, combined_comp_bending_calc.Results_Util));
                    AddResult(GetSubcalculationResult(overall_comp_bending_calc, overall_comp_bending_calc.Results_Util));
                    AddResult(GetSubcalculationResult(ltb_comp_bending_calc, ltb_comp_bending_calc.Results_Util));
                }
                else
                {
                    AddSubHeader("Compression and bending cross sectional strength I shaped class 3", true, ReportSubHeading.SubheadingLevel.Level1);

                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingCrossSectionalStrength combined_comp_bending_calc;
                    combined_comp_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingCrossSectionalStrength(calculationID, C_f.Value, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value, F_y.Value, E.Value, A.Value, omega_1_x.Value, omega_1_y.Value, L_x.Value, L_y.Value, r_x.Value, r_y.Value, I_x.Value, I_y.Value);
                    DeclareSubcalculation(combined_comp_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    cross_section_util = combined_comp_bending_calc.Results_Util.ValueTyped;
                    AddResult(GetSubcalculationResult(combined_comp_bending_calc, combined_comp_bending_calc.Results_Util));

                    AddSubHeader("Compression and bending overall member strength I shaped class 3", true, ReportSubHeading.SubheadingLevel.Level1);
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingOverallMemberStrength overall_comp_bending_calc;
                    overall_comp_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingOverallMemberStrength(calculationID, C_f.Value, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value, F_y.Value, E.Value, A.Value,input_n_category,omega_1_x.Value, omega_1_y.Value, L_x.Value, L_y.Value, r_x.Value, r_y.Value, I_x.Value, I_y.Value,input_frame_type);
                    DeclareSubcalculation(overall_comp_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    overall_util = overall_comp_bending_calc.Results_Util.ValueTyped;
                    
                    AddSubHeader("Compression and bending lateral torsional buckling strength I shaped class 3", true, ReportSubHeading.SubheadingLevel.Level1);
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingLTBStrength ltb_comp_bending_calc;
                    ltb_comp_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.CompressionAndBendingLTBStrength(calculationID, C_f.Value, M_f_x.Value, M_f_y.Value, M_r_x.Value, M_r_y.Value, F_y.Value, E.Value, A.Value, input_n_category, omega_1_x.Value, omega_1_y.Value, K_x.Value, K_y.Value, L_x.Value, L_y.Value,L_z.Value ,r_x.Value, r_y.Value, I_x.Value, I_y.Value, C_w.Value, G.Value, J.Value, x_0.Value, y_0.Value, input_frame_type);
                    DeclareSubcalculation(ltb_comp_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    ltb_util = ltb_comp_bending_calc.Results_Util.ValueTyped;                    
                    AddResult(GetSubcalculationResult(combined_comp_bending_calc, combined_comp_bending_calc.Results_Util));
                    AddResult(GetSubcalculationResult(overall_comp_bending_calc, overall_comp_bending_calc.Results_Util));
                    AddResult(GetSubcalculationResult(ltb_comp_bending_calc, ltb_comp_bending_calc.Results_Util));
                }
            }

            //Combined tension and bending
            else if((T_f > 0) && (M_f_x > 0||M_f_y > 0))
            {
            AddSubHeader("Combined axial tension and bending", true, ReportSubHeading.SubheadingLevel.Level1);
                if ((flange_class == SectionClass.Class_1 || flange_class == SectionClass.Class_2) && (web_class == SectionClass.Class_1 || web_class == SectionClass.Class_2))
                {
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.MemberUtilisationTensionAndBendingClass1_2 combined_tens_bending_calc;
                    combined_tens_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.MemberUtilisationTensionAndBendingClass1_2(calculationID, input_lateral_support, T_f.Value, M_f_x.Value, M_f_y.Value, Z_x.Value, Z_y.Value, A.Value, T_r.Value, M_r_x.Value, M_r_y.Value);
                    DeclareSubcalculation(combined_tens_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    tens_bending_util = combined_tens_bending_calc.Results_MaxUtil.ValueTyped;
                    AddResult(GetSubcalculationResult(combined_tens_bending_calc, combined_tens_bending_calc.Results_MaxUtil));
                }
                else
                {
                    DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.MemberUtilisationTensionAndBendingClass3_4 combined_tens_bending_calc;
                    combined_tens_bending_calc = new DesignCheck2.Structural.Steel.CSA.S16_14.Members.Combinations.MemberUtilisationTensionAndBendingClass3_4(calculationID, input_lateral_support, T_f.Value, M_f_x.Value, M_f_y.Value, S_x.Value, S_y.Value, A.Value, T_r.Value, M_r_x.Value, M_r_y.Value);
                    DeclareSubcalculation(combined_tens_bending_calc, false);
                    if (this.ErrorsExist)
                        return;
                    tens_bending_util = combined_tens_bending_calc.Results_MaxUtil.ValueTyped;
                    AddResult(GetSubcalculationResult(combined_tens_bending_calc, combined_tens_bending_calc.Results_MaxUtil));
                }
            }


            //summary
            AddSubHeader("Capacity check summary", true, ReportSubHeading.SubheadingLevel.Level1);
            AddBodyText("Summary table of demands, capacities and utilisation ratios for axial forces, moments and shear forces:");

            //System.Data.DataTable empty = new System.Data.DataTable();
            //AddDataTable(empty);

            System.Data.DataTable summary_table_forces = new System.Data.DataTable();

            summary_table_forces.Columns.Add("Summary", typeof(string));
            summary_table_forces.Columns.Add("Compression, Cf/Cr", typeof(double));
            summary_table_forces.Columns.Add("Tension, Tf/Tr", typeof(double));
            summary_table_forces.Columns.Add("Strong Axis Moment, Mfx/Mrx", typeof(double));
            summary_table_forces.Columns.Add("Weak Axis Moment, Mfy/Mry", typeof(double));
            summary_table_forces.Columns.Add("Shear in X-Axis Direction, Vfx/Vrx", typeof(double));
            summary_table_forces.Columns.Add("Shear in Y-Axis Direction, Vfy/Vry", typeof(double));
            summary_table_forces.Rows.Add("<b>Demand (kN)<b>", C_f.Value, T_f.Value, M_f_x.Value, M_f_y.Value, V_f_x.Value, V_f_y.Value);
            summary_table_forces.Rows.Add("<b>Capacity (kN)<b>", Math.Round(C_r.Value, 3), Math.Round(T_r.Value, 3), Math.Round(M_r_x.Value, 3), Math.Round(M_r_y.Value, 3), Math.Round(V_r_x.Value, 3), Math.Round(V_r_y.Value, 3));
            summary_table_forces.Rows.Add("<b>Utilisation<b>", Math.Round(C_f.Value / C_r.Value, 3), Math.Round(T_f.Value / T_r.Value, 3), Math.Round(M_f_x.Value / M_r_x.Value, 3), Math.Round(M_f_y.Value / M_r_y.Value, 3), Math.Round(V_f_x.Value / V_r_x.Value, 3), Math.Round(V_f_y.Value / V_r_y.Value, 3));
            AddDataTable(summary_table_forces);


            AddBodyText("Summary table of utilisation ratios for combined axial compression and bending, biaxial bending and combined axial tension and bending:");
            System.Data.DataTable summary_table_util = new System.Data.DataTable();
            summary_table_util.Columns.Add("Cross Section Utilisation", typeof(double));
            summary_table_util.Columns.Add("Overall Member Strength Utilisation", typeof(double));
            summary_table_util.Columns.Add("Lat Torsional Buckling Utilisation", typeof(double));
            summary_table_util.Columns.Add("Biaxial Bending Utilisation", typeof(double));
            summary_table_util.Columns.Add("Tension and Bending Utilisation", typeof(double));
            summary_table_util.Columns.Add("Max Utilisation", typeof(double));

            Double max_util = Math.Max(tens_bending_util, Math.Max(biaxial_util, Math.Max(cross_section_util, Math.Max(overall_util, ltb_util))));
            summary_table_util.Rows.Add(Math.Round(cross_section_util, 3), Math.Round(overall_util, 3), Math.Round(ltb_util, 3), Math.Round(biaxial_util, 3), Math.Round(tens_bending_util, 3), Math.Round(max_util, 3));

            AddDataTable(summary_table_util);


            
            AddBibliography();
        }
    }
}