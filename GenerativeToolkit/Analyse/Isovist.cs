﻿using Autodesk.DesignScript.Geometry;
using Autodesk.DesignScript.Runtime;
using Autodesk.GenerativeToolkit.Analyse;
using Autodesk.GenerativeToolkit.Utilities.GraphicalGeometry;
using Dynamo.Graph.Nodes;
using GenerativeToolkit.Graphs.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPoint = Autodesk.DesignScript.Geometry.Point;
using graphs = GenerativeToolkit.Graphs.Graphs;

namespace Autodesk.GenerativeToolkit.Analyse
{
    public static class Isovist
    {
        #region Public Methods

        /// <summary>
        /// Returns a surface representing the Isovist area visible from 
        /// the given point.
        /// </summary>
        /// <param name="baseGraph">Base Graph</param>
        /// <param name="point">Origin point</param>
        /// <returns name="isovist">Surface representing the isovist area</returns>
        [NodeCategory("Actions")]
        public static Surface IsovistFromPoint(List<Polygon> boundary, List<Polygon> internals, DSPoint point)
        {
            BaseGraph baseGraph = BaseGraph.ByBoundaryAndInternalPolygons(boundary,internals);

            if (baseGraph == null) { throw new ArgumentNullException("graph"); }
            if (point == null) { throw new ArgumentNullException("point"); }

            gVertex origin = gVertex.ByCoordinates(point.X, point.Y, point.Z);

            List<gVertex> vertices = graphs.VisibilityGraph.VertexVisibility(origin, baseGraph.graph);
            List<DSPoint> points = vertices.Select(v => Points.ToPoint(v)).ToList();
            // TODO: Implement better way of checking if polygon is self intersectingç

            Polygon polygon = Polygon.ByPoints(points);

            if (polygon.SelfIntersections().Length > 0)
            {
                points.Add(point);
                polygon = Polygon.ByPoints(points);

            }

            return Surface.ByPatch(polygon);
        }

        #endregion
    }
}