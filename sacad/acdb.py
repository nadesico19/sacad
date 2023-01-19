# Copyright (c) 2022 Chin Ako <nadesico19@gmail.com>
# sacad is licensed under Mulan PSL v2.
# You can use this software according to the terms and conditions of the Mulan
# PSL v2.
# You may obtain a copy of Mulan PSL v2 at:
#          http://license.coscl.org.cn/MulanPSL2
# THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND,
# EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT,
# MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
# See the Mulan PSL v2 for more details.

"""acdb: AcDb stands for `Autodesk.AutoCAD.DatabaseServices`."""

from dataclasses import dataclass, field
from enum import IntEnum
from typing import Optional, List, Dict

from sacad.accm import Color
from sacad.acge import Vector2d, Vector3d
from sacad.jsonify import Jsonify
from sacad.util import csharp_polymorphic_type

__all__ = [
    'MODEL_SPACE',
    # 'PAPER_SPACE',
    'ObjectId',
    'Database',
    'LineWeight',
    'DBObject',
    'Entity',
    # 'BlockReference',
    # 'DBText',
    # 'Hatch',
    'Curve',
    # 'Arc',
    # 'Circle',
    # 'Ellipse',
    # 'Leader',
    # 'Line',
    'Vertex',
    'Polyline',
    # 'Polyline2d',
    # 'Polyline3d',
    # 'Ray',
    # 'Spline',
    # 'Xline',
    # 'Dimension',
    # 'AlignedDimension',
    # 'ArcDimension',
    # 'DiametricDimension',
    # 'LineAngularDimension2',
    # 'Point3AngularDimension',
    # 'RadialDimension',
    # 'RadialDimensionLarge',
    # 'RotatedDimension',
    'SymbolTableRecord',
    'BlockTableRecord',
    # 'DimStyleTableRecord',
    'LayerTableRecord',
    # 'LinetypeTableRecord',
    # 'TextStyleTableRecord',
]

MODEL_SPACE = '*MODEL_SPACE'
# PAPER_SPACE = '*PAPER_SPACE'

ObjectId = Optional[int]


class LineWeight(IntEnum):
    BY_BLOCK = -2
    BY_LAYER = -1
    BY_LINE_WEIGHT_DEFAULT = -3
    LINE_WEIGHT_000 = 0
    LINE_WEIGHT_005 = 5
    LINE_WEIGHT_009 = 9
    LINE_WEIGHT_013 = 13
    LINE_WEIGHT_015 = 15
    LINE_WEIGHT_018 = 0x12
    LINE_WEIGHT_020 = 20
    LINE_WEIGHT_025 = 0x19
    LINE_WEIGHT_030 = 30
    LINE_WEIGHT_035 = 0x23
    LINE_WEIGHT_040 = 40
    LINE_WEIGHT_050 = 50
    LINE_WEIGHT_053 = 0x35
    LINE_WEIGHT_060 = 60
    LINE_WEIGHT_070 = 70
    LINE_WEIGHT_080 = 80
    LINE_WEIGHT_090 = 90
    LINE_WEIGHT_100 = 100
    LINE_WEIGHT_106 = 0x6a
    LINE_WEIGHT_120 = 120
    LINE_WEIGHT_140 = 140
    LINE_WEIGHT_158 = 0x9e
    LINE_WEIGHT_200 = 200
    LINE_WEIGHT_211 = 0xd3


@dataclass
class DimStyleCommon:
    """Common attributes shared between DimStyleTableRecord and Dimension."""

    # Controls the number of precision places displayed in angular dimensions.
    #          Type: Integer
    #      Saved in: Drawing
    # Initial value: 0
    #            -1: Angular dimensions display the number of decimal places
    #                specified by DIMDEC.
    #           0-8: Specifies the number of decimal places displayed in angular
    #                dimensions (independent of DIMDEC)
    dimadec: Optional[int] = None

    # Controls the display of alternate units in dimensions.
    #          Type: Switch
    #      Saved in: Drawing
    # Initial value: OFF
    #           OFF: Disables alternate units
    #            ON: Enables alternate units
    dimalt: Optional[bool] = None

    # Controls the number of decimal places in alternate units.
    #          Type: Integer
    #      Saved in: Drawing
    # Initial value: 2 (imperial) or 3 (metric)
    # If DIMALT is turned on, DIMALTD sets the number of digits displayed to
    # the right of the decimal point in the alternate measurement.
    dimaltd: Optional[int] = None

    # Controls the multiplier for alternate units.
    #          Type: Real
    #      Saved in: Drawing
    # Initial value: 25.4000 (imperial) or 0.0394 (metric)
    # If DIMALT is turned on, DIMALTF multiplies linear dimensions by a
    # factor to produce a value in an alternate system of measurement. The
    # initial value represents the number of millimeters in an inch.
    dimaltf: Optional[float] = None

    # Rounds off the alternate dimension units.
    #          Type: Real
    #      Saved in: Drawing
    # Initial value: 0.0000
    dimaltrnd: Optional[float] = None

    dimalttd: Optional[int] = None
    dimalttz: Optional[int] = None
    dimaltu: Optional[int] = None
    dimaltz: Optional[int] = None
    dimapost: Optional[str] = None
    dimarcsym: Optional[int] = None
    dimasz: Optional[float] = None
    dimatfit: Optional[int] = None
    dimaunit: Optional[int] = None
    dimazin: Optional[int] = None
    dimblk: Optional[str] = None
    dimblk1: Optional[str] = None
    dimblk2: Optional[str] = None
    dimcen: Optional[float] = None
    dimclrd: Optional[Color] = None
    dimclre: Optional[Color] = None
    dimclrt: Optional[Color] = None
    dimdec: Optional[int] = None
    dimdle: Optional[float] = None
    dimdli: Optional[float] = None
    dimdsep: Optional[str] = None
    dimexe: Optional[float] = None
    dimexo: Optional[float] = None
    dimfrac: Optional[int] = None
    dimfxlen: Optional[float] = None
    dimfxlenOn: Optional[bool] = None
    dimgap: Optional[float] = None
    dimjogang: Optional[float] = None
    dimjust: Optional[int] = None
    dimldrblk: Optional[str] = None
    dimlfac: Optional[float] = None
    dimlim: Optional[bool] = None
    dimltex1: Optional[str] = None
    dimltex2: Optional[str] = None
    dimltype: Optional[str] = None
    dimlunit: Optional[int] = None
    dimlwd: Optional[LineWeight] = None
    dimlwe: Optional[LineWeight] = None
    dimpost: Optional[str] = None
    dimrnd: Optional[float] = None
    dimsah: Optional[bool] = None
    dimscale: Optional[float] = None
    dimsd1: Optional[bool] = None
    dimsd2: Optional[bool] = None
    dimse1: Optional[bool] = None
    dimse2: Optional[bool] = None
    dimsoxd: Optional[bool] = None
    dimtad: Optional[int] = None
    dimtdec: Optional[int] = None
    dimtfac: Optional[float] = None
    dimtfill: Optional[int] = None
    dimtfillclr: Optional[Color] = None
    dimtih: Optional[bool] = None
    dimtix: Optional[bool] = None
    dimtm: Optional[float] = None
    dimtmove: Optional[int] = None
    dimtofl: Optional[bool] = None
    dimtoh: Optional[bool] = None
    dimtol: Optional[bool] = None
    dimtolj: Optional[int] = None
    dimtp: Optional[float] = None
    dimtsz: Optional[float] = None
    dimtvp: Optional[float] = None
    dimtxt: Optional[float] = None
    dimtzin: Optional[int] = None
    dimupt: Optional[bool] = None
    dimzin: Optional[int] = None


@dataclass
class DBObject(Jsonify):
    id: ObjectId = None


@dataclass
class Entity(DBObject):
    layer: Optional[str] = None
    linetype: Optional[str] = None
    linetype_scale: Optional[float] = None
    line_weight: Optional[LineWeight] = None
    visible: Optional[bool] = None
    # TODO


# @dataclass
# class BlockReference(Entity):
#     pass


# @dataclass
# class DBText(Entity):
#     pass


# @dataclass
# class Hatch(Entity):
#     pass


@dataclass
class Curve(Entity):
    pass


# @dataclass
# class Arc(Curve):
#     pass


# @dataclass
# class Circle(Curve):
#     pass


# @dataclass
# class Ellipse(Curve):
#     pass


# @dataclass
# class Leader(Curve):
#     pass


# @dataclass
# class Line(Curve):
#     start_point: Optional[Vector3d] = None
#     end_point: Optional[Vector3d] = None
#     thickness: Optional[float] = None


@dataclass
class Vertex(Jsonify):
    point: Vector2d
    bulge: Optional[float] = None
    start_width: Optional[float] = None
    end_width: Optional[float] = None


@csharp_polymorphic_type("SacadMgd.Polyline, SacadMgd")
@dataclass
class Polyline(Curve):
    closed: Optional[bool] = None
    constant_width: Optional[float] = None
    elevation: Optional[float] = None
    normal: Optional[Vector3d] = None
    thickness: Optional[float] = None
    vertices: Optional[List[Vertex]] = None


# @dataclass
# class Polyline2d(Curve):
#     pass


# @dataclass
# class Polyline3d(Curve):
#     pass


# @dataclass
# class Ray(Curve):
#     pass


# @dataclass
# class Spline(Curve):
#     pass


# @dataclass
# class Xline(Curve):
#     pass


# @dataclass
# class Dimension(Entity, DimStyleCommon):
#     text_position: Optional[Vector3d] = None
#     text_rotation: Optional[float] = None
#     # TODO


# @dataclass
# class AlignedDimension(Dimension):
#     pass


# @dataclass
# class ArcDimension(Dimension):
#     pass


# @dataclass
# class DiametricDimension(Dimension):
#     pass


# @dataclass
# class LineAngularDimension2(Dimension):
#     pass


# @dataclass
# class Point3AngularDimension(Dimension):
#     pass


# @dataclass
# class RadialDimension(Dimension):
#     pass


# @dataclass
# class RadialDimensionLarge(Dimension):
#     pass


# @dataclass
# class RotatedDimension(Dimension):
#     pass


@dataclass
class SymbolTableRecord(DBObject):
    name: Optional[str] = None


@csharp_polymorphic_type("SacadMgd.BlockTableRecord, SacadMgd")
@dataclass
class BlockTableRecord(SymbolTableRecord):
    entities: List[Entity] = field(default_factory=list)


# @dataclass
# class DimStyleTableRecord(SymbolTableRecord, DimStyleCommon):
#     pass


@csharp_polymorphic_type("SacadMgd.LayerTableRecord, SacadMgd")
@dataclass
class LayerTableRecord(SymbolTableRecord):
    color: Optional[Color] = None


# @dataclass
# class LinetypeSegment:
#     dash_length: Optional[float] = None
#     shape_is_ucs_oriented: Optional[bool] = None
#     shape_number: Optional[int] = None
#     shape_offset: Optional[Vector2d] = None
#     shape_rotation: Optional[float] = None
#     shape_scale: Optional[float] = None
#     shape_style: Optional[str] = None
#     text: Optional[str] = None


# @dataclass
# class LinetypeTableRecord(SymbolTableRecord):
#     comments: Optional[str] = None
#     segments: Optional[List[LinetypeSegment]] = None
#
#     # Accesses the alignment type for the LinetypeTableRecord. If ScaledToFit is
#     # true, the alignment wll be "scaled to fit" (equivalent to an 'S' in the
#     # alignment field of the linetype definition). If ScaledToFit is false, the
#     # alignment will not be "scaled to fit" (equivalent to an 'A' in the
#     # alignment field of the linetype definition).
#     is_scaled_to_fit: Optional[bool] = None
#
#     # Accesses the length (in AutoCAD drawing units--the pattern will appear
#     # this length when the linetype scale is 1.0) of the pattern in the
#     # LinetypeTableRecord. The pattern length is the total length of all dashes
#     # (including pen up spaces). Embedded shapes or text strings do not add to
#     # the pattern length because they are overlaid and do not interrupt the
#     # actual dash pattern. For more information on linetype definitions, see the
#     # "Linetypes" section of the AutoCAD Customization Guide.
#     pattern_length: Optional[float] = None
#
#     # This function is obsolete and will be eliminated in a future release of
#     # ObjectARX. Please use Comments instead.
#     ascii_description: Optional[str] = None


# @dataclass
# class TextStyleTableRecord(SymbolTableRecord):
#     pass


@dataclass
class Database(Jsonify):
    blocktable: Dict[str, BlockTableRecord] = field(default_factory=dict)
    layertable: Dict[str, LayerTableRecord] = field(default_factory=dict)

    def get_blocktable(self, name):
        if name not in self.blocktable:
            self.blocktable[name] = BlockTableRecord()
        return self.blocktable[name]
