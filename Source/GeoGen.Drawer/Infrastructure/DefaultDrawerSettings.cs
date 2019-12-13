using GeoGen.Infrastructure;
using System.Collections.Generic;

namespace GeoGen.Drawer
{
    /// <summary>
    /// The default <see cref="DrawerSettings"/>
    /// </summary>
    public class DefaultDrawerSettings : DrawerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultDrawerSettings"/> class
        /// </summary>
        public DefaultDrawerSettings() : base
        (
            loggingSettings: new LoggingSettings
            (
                loggers: new List<BaseLoggerSettings>
                {
                    // Console logger
                    new ConsoleLoggerSettings
                    (
                        includeLoggingOrigin: false,
                        includeTime: false,
                        logOutputLevel: LogOutputLevel.Info
                    ),

                    // File logger
                    new FileLoggerSettings
                    (
                        includeLoggingOrigin: true,
                        includeTime: true,
                        logOutputLevel: LogOutputLevel.Debug,
                        fileLogPath: "log.txt"
                    )
                }
            ),
            metapostDrawerSettings: new MetapostDrawerSettings
            (
                drawingData: new MetapostDrawingData
                (
                    scaleVariable: "u",
                    shiftLength: 0.4,
                    boundingBoxOffset: 1,
                    pointLabelMacro: "LabelPoint",
                    pointMarkMacros: new Dictionary<ObjectDrawingStyle, string>
                    {
                        { ObjectDrawingStyle.AuxiliaryObject, "PointMarkAuxiliaryStyle" },
                        { ObjectDrawingStyle.NormalObject, "PointMarkNormalStyle" },
                        { ObjectDrawingStyle.TheoremObject, "PointMarkTheoremStyle" }
                    },
                    lineSegmentMacros: new Dictionary<ObjectDrawingStyle, string>
                    {
                        { ObjectDrawingStyle.AuxiliaryObject, "LineSegmentAuxiliaryStyle" },
                        { ObjectDrawingStyle.NormalObject, "LineSegmentNormalStyle" },
                        { ObjectDrawingStyle.TheoremObject, "LineSegmentTheoremStyle" }
                    },
                    circleMacros: new Dictionary<ObjectDrawingStyle, string>
                    {
                        { ObjectDrawingStyle.AuxiliaryObject, "CircleAuxiliaryStyle" },
                        { ObjectDrawingStyle.NormalObject, "CircleNormalStyle" },
                        { ObjectDrawingStyle.TheoremObject, "CircleTheoremStyle" }
                    }
                ),
                metapostCodeFilePath: "figures.mp",
                metapostMacrosLibraryPath: "macros.mp",
                compilationCommand: (program: "mpost", arguments: "-interaction=nonstopmode -s prologues=3"),
                postcompilationCommand: "post-compile.bat",
                logCommandOutput: true,
                numberOfPictures: 20
            ),
            drawingRulesProviderSettings: new DrawingRulesProviderSettings(filePath: "..\\..\\..\\drawing_rules.txt")
        )
        { }
    }
}
