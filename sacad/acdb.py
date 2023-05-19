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
from typing import Dict, List, Optional

from sacad.accm import Color
from sacad.acge import Matrix3d, Number, Vector2d, Vector3d
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
    'Arc',
    # 'Circle',
    # 'Ellipse',
    # 'Leader',
    'Line',
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
    'LinetypeSegment',
    'LinetypeTableRecord',
    'FontDescriptor',
    'TextStyleTableRecord',
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
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #  -1   | Angular dimensions display the number of decimal places specified
    #       | by DIMDEC.
    #  0-8  | Specifies the number of decimal places displayed in angular
    #       | dimensions (independent of DIMDEC)
    dimadec: Optional[int] = None

    # Controls the display of alternate units in dimensions.
    #
    # Type          : Switch
    # Saved in      : Drawing
    # Initial value : OFF
    #
    # Value | Description
    # ------+------------
    #  OFF  | Disables alternate units
    #  ON   | Enables alternate units
    dimalt: Optional[bool] = None

    # Controls the number of decimal places in alternate units.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 2 (imperial) or 3 (metric)
    #
    # If DIMALT is turned on, DIMALTD sets the number of digits displayed to the
    # right of the decimal point in the alternate measurement.
    dimaltd: Optional[int] = None

    # Controls the multiplier for alternate units.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 25.4000 (imperial) or 0.0394 (metric)
    #
    # If DIMALT is turned on, DIMALTF multiplies linear dimensions by a factor
    # to produce a value in an alternate system of measurement. The initial
    # value represents the number of millimeters in an inch.
    dimaltf: Optional[float] = None

    # Rounds off the alternate dimension units.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.0000
    dimaltrnd: Optional[float] = None

    # Sets the number of decimal places for the tolerance values in the
    # alternate units of a dimension.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 2(imperial) or 3(metric)
    dimalttd: Optional[int] = None

    # Controls suppression of zeros in tolerance values.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #   0   | Suppresses zero feet and precisely zero inches
    #   1   | Includes zero feet and precisely zero inches
    #   2   | Includes zero feet and suppresses zero inches
    #   3   | Includes zero inches and suppresses zero feet
    #
    # To suppress leading or trailing zeros, add the following values to one of
    # the preceding values:
    #
    # Value | Description
    # ------+------------
    #   4   | Suppresses leading zeros
    #   8   | Suppresses trailing zeros
    dimalttz: Optional[int] = None

    # Sets the units format for alternate units of all dimension substyles
    # except Angular.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 2
    #
    # Value | Description
    # ------+------------
    #   1   | Scientific
    #   2   | Decimal
    #   3   | Engineering
    #   4   | Architectural(stacked)
    #   5   | Fractional(stacked)
    #   6   | Architectural
    #   7   | Fractional
    #   8   | Microsoft Windows Desktop(decimal format using Control Panel
    #       | settings for decimal separator and number grouping symbols)
    dimaltu: Optional[int] = None

    # Controls the suppression of zeros for alternate unit dimension values.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # DIMALTZ values 0 - 3 affect feet - and -inch dimensions only.
    #
    # Value | Description
    # ------+------------
    #   0   | Suppresses zero feet and precisely zero inches
    #   1   | Includes zero feet and precisely zero inches
    #   2   | Includes zero feet and suppresses zero inches
    #   3   | Includes zero inches and suppresses zero feet
    #   4   | Suppresses leading zeros in decimal dimensions (for example,
    #       | 0.5000 becomes .5000)
    #   8   | Suppresses trailing zeros in decimal dimensions (for example,
    #       | 12.5000 becomes 12.5)
    #  12   | Suppresses both leading and trailing zeros (for example, 0.5000
    #       | becomes .5)
    dimaltz: Optional[int] = None

    # Specifies a text prefix or suffix (or both) to the alternate dimension
    # measurement for all types of dimensions except angular.
    #
    # Type          : String
    # Saved in      : Drawing
    # Initial value : ""
    #
    # For instance, if the current units are Architectural, DIMALT is on,
    # DIMALTF is 25.4 (the number of millimeters per inch), DIMALTD is 2, and
    # DIMAPOST is set to "mm", a distance of 10 units would be displayed as
    # 10"[254.00mm].
    #
    # To turn off an established prefix or suffix (or both), set it to a single
    # period (.).
    dimapost: Optional[str] = None

    # Controls display of the arc symbol in an arc length dimension.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #   0   | Places arc length symbols before the dimension text
    #   1   | Places arc length symbols above the dimension text
    #   2   | Suppresses the display of arc length symbols
    dimarcsym: Optional[int] = None

    # Controls the size of dimension line and leader line arrowheads. Also
    # controls the size of hook lines.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.1800 (imperial) or 2.5000 (metric)
    #
    # Multiples of the arrowhead size determine whether
    # dimension lines and text should fit between the extension lines. DIMASZ
    # is also used to scale arrowhead blocks if set by DIMBLK. DIMASZ has no
    # effect when DIMTSZ is other than zero.
    dimasz: Optional[float] = None

    # Determines how dimension text and arrows are arranged when space is not
    # sufficient to place both within the extension lines.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 3
    #
    # Value | Description
    # ------+------------
    #   0   | Places both text and arrows outside extension lines
    #   1   | Moves arrows first, then text
    #   2   | Moves text first, then arrows
    #   3   | Moves either text or arrows, whichever fits best
    #
    # A leader is added to moved dimension text when DIMTMOVE is set to 1.
    dimatfit: Optional[int] = None

    # Sets the units format for angular dimensions.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #   0   | Decimal degrees
    #   1   | Degrees/minutes/seconds
    #   2   | Gradians
    #   3   | Radians
    dimaunit: Optional[int] = None

    # Suppresses zeros for angular dimensions.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #   0   | Displays all leading and trailing zeros
    #   1   | Suppresses leading zeros in decimal dimensions (for example,
    #       | 0.5000 becomes .5000)
    #   2   | Suppresses trailing zeros in decimal dimensions (for example,
    #       | 12.5000 becomes 12.5)
    #   3   | Suppresses leading and trailing zeros (for example, 0.5000 becomes
    #       | .5)
    dimazin: Optional[int] = None

    # Sets the arrowhead block displayed at the ends of dimension lines.
    #
    # Type          : String
    # Saved in      : Drawing
    # Initial value : ""
    #
    # To return to the default, closed-filled arrowhead display, enter a single
    # period (.). Arrowhead block entries and the names used to select them in
    # the New, Modify, and Override Dimension Style dialog boxes are shown
    # below. You can also enter the names of user-defined arrowhead blocks.
    #
    # Note: Annotative blocks cannot be used as custom arrowheads for dimensions
    # or leaders.
    #
    #          Value | Description
    # ---------------+------------
    #             "" | closed filled
    #         "_DOT" | dot
    #    "_DOTSMALL" | dot small
    #    "_DOTBLANK" | dot blank
    #      "_ORIGIN" | origin indicator
    #     "_ORIGIN2" | origin indicator 2
    #        "_OPEN" | open
    #      "_OPEN90" | right angle
    #      "_OPEN30" | open 30
    #      "_CLOSED" | closed
    #       "_SMALL" | dot small blank
    #        "_NONE" | none
    #     "_OBLIQUE" | oblique
    #   "_BOXFILLED" | box filled
    #    "_BOXBLANK" | box
    # "_CLOSEDBLANK" | closed blank
    # "_DATUMFILLED" | datum triangle filled
    #  "_DATUMBLANK" | datum triangle
    #    "_INTEGRAL" | integral
    #    "_ARCHTICK" | architectural tick
    dimblk: Optional[str] = None

    # Sets the arrowhead for the first end of the dimension line when DIMSAH is
    # on.
    #
    # Type          : String
    # Saved in      : Drawing
    # Initial value : ""
    #
    # To return to the default, closed-filled arrowhead display, enter a single
    # period (.). For a list of arrowheads, see DIMBLK.
    #
    # Note: Annotative blocks cannot be used as custom arrowheads for dimensions
    # or leaders.
    dimblk1: Optional[str] = None

    # Sets the arrowhead for the second end of the dimension line when DIMSAH is
    # on.
    #
    # Type          : String
    # Saved in      : Drawing
    # Initial value : ""
    #
    # To return to the default, closed-filled arrowhead display, enter a single
    # period (.). For a list of arrowhead entries, see DIMBLK.
    #
    # Note: Annotative blocks cannot be used as custom arrowheads for dimensions
    # or leaders.
    dimblk2: Optional[str] = None

    # Controls drawing of circle or arc center marks and centerlines by the
    # DIMCENTER, DIMDIAMETER, and DIMRADIUS commands.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.0900 (imperial) or 2.5000 (metric)
    #
    # For DIMDIAMETER and DIMRADIUS, the center mark is drawn only if you place
    # the dimension line outside the circle or arc.
    #
    # Value | Description
    # ------+------------
    #   0   | No center marks or lines are drawn
    #  <0   | Centerlines are drawn
    #  >0   | Center marks are drawn
    #
    # The absolute value specifies the size of the center mark or centerline.
    #
    # The size of the centerline is the length of the centerline segment that
    # extends outside the circle or arc. It is also the size of the gap between
    # the center mark and the start of the centerline.
    #
    # The size of the center mark is the distance from the center of the circle
    # or arc to the end of the center mark.
    dimcen: Optional[float] = None

    # Assigns colors to dimension lines, arrowheads, and dimension leader lines.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Also controls the color of leader lines created with the LEADER command.
    # Color numbers are displayed in the Select Color dialog box. For BYBLOCK,
    # enter 0. For BYLAYER, enter 256.
    dimclrd: Optional[Color] = None

    # Assigns colors to extension lines, center marks, and centerlines.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Color numbers are displayed in the Select Color dialog box. For BYBLOCK,
    # enter 0. For BYLAYER, enter 256.
    dimclre: Optional[Color] = None

    # Assigns colors to dimension text.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # The color can be any valid color number.
    dimclrt: Optional[Color] = None

    # Sets the number of decimal places displayed for the primary units of a
    # dimension.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 4 (imperial) or 2 (metric)
    #
    # The precision is based on the units or angle format you have selected.
    # Specified value is applied to angular dimensions when DIMADEC is set to
    # -1.
    dimdec: Optional[int] = None

    # Sets the distance the dimension line extends beyond the extension line
    # when oblique strokes are drawn instead of arrowheads.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.0000
    dimdle: Optional[float] = None

    # Controls the spacing of the dimension lines in baseline dimensions.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.3800 (imperial) or 3.7500 (metric)
    #
    # Each dimension line is offset from the previous one by this amount, if
    # necessary, to avoid drawing over it. Changes made with DIMDLI are not
    # applied to existing dimensions.
    dimdli: Optional[float] = None

    # Specifies a single-character decimal separator to use when creating
    # dimensions whose unit format is decimal.
    #
    # Type          : Single-character
    # Saved in      : Drawing
    # Initial value : . (imperial) or , (metric)
    #
    # When prompted, enter a single character at the Command prompt. If
    # dimension units is set to Decimal, the DIMDSEP character is used instead
    # of the default decimal point. If DIMDSEP is set to NULL (default value,
    # reset by entering a period), the decimal point is used as the dimension
    # separator.
    dimdsep: Optional[str] = None

    # Specifies how far to extend the extension line beyond the dimension line.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.1800 (imperial) or 1.2500 (metric)
    dimexe: Optional[float] = None

    # Specifies how far extension lines are offset from origin points.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.0625 (imperial) or 0.6250 (metric)
    # With fixed-length extension lines, this value determines the minimum
    # offset.
    dimexo: Optional[float] = None

    # Sets the fraction format when DIMLUNIT is set to 4 (Architectural) or 5
    # (Fractional).
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #   0   | Horizontal stacking
    #   1   | Diagonal stacking
    #   2   | Not stacked (for example, 1/2)
    dimfrac: Optional[int] = None

    # The fixed extension lines value.
    dimfxlen: Optional[float] = None

    # A value that indicates whether the fixed extension lines are on
    dimfxlenOn: Optional[bool] = None

    # Sets the distance around the dimension text when the dimension line breaks
    # to accommodate dimension text.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 0.0900 (imperial) or 0.6250 (metric)
    #
    # Also sets the gap between annotation and a hook line created with the
    # LEADER command. If you enter a negative value, DIMGAP places a box around
    # the dimension text.
    #
    # The value of DIMGAP is also used as the minimum length of each segment of
    # the dimension line. To locate the components of a linear dimension within
    # the extension lines, enough space must be available for both arrowheads
    # (2 x DIMASZ), both dimension line segments (2 x DIMGAP), a gap on either
    # side of the dimension text (another 2 x DIMGAP), and the length of the
    # dimension text, which depends on its size and number of decimal places
    # displayed.
    dimgap: Optional[float] = None

    # Determines the angle of the transverse segment of the dimension line in a
    # jogged radius dimension.
    #
    # Type          : Real
    # Saved in      : Drawing
    # Initial value : 45
    # Jogged radius dimensions are often created when the center point is
    # located off the page. Valid settings range is 5 to 90.
    dimjogang: Optional[float] = None

    # Controls the horizontal positioning of dimension text.
    #
    # Type          : Integer
    # Saved in      : Drawing
    # Initial value : 0
    #
    # Value | Description
    # ------+------------
    #   0   | Positions the text above the dimension line and center-justifies
    #       | it between the extension lines
    #   1   | Positions the text next to the first extension line
    #   2   | Positions the text next to the second extension line
    #   3   | Positions the text above and aligned with the first extension line
    #   4   | Positions the text above and aligned with the second extension
    #       | line
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
    color: Optional[Color] = None
    color_index: Optional[int] = None
    layer: Optional[str] = None
    linetype: Optional[str] = None
    linetype_scale: Optional[float] = None
    line_weight: Optional[LineWeight] = None
    visible: Optional[bool] = None
    transform: Optional[Matrix3d] = None

    def transform_by(self, matrix: Matrix3d):
        if self.transform is None:
            self.transform = matrix
        else:
            pass  # TODO


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


@dataclass
class Arc(Curve):
    center: Optional[Vector3d] = None
    normal: Optional[Vector3d] = None
    radius: Optional[float] = None
    start_angle: Optional[float] = None
    end_angle: Optional[float] = None
    thickness: Optional[float] = None

    @staticmethod
    def new(center_x: Number, center_y: Number, radius: Number,
            start_angle: Number, end_angle: Number, **kwargs):
        return Arc(center=Vector3d(center_x, center_y), radius=radius,
                   start_angle=start_angle, end_angle=end_angle, **kwargs)

    @staticmethod
    def new_vec2(center: Vector2d, radius: Number,
                 start_angle: Number, end_angle: Number, **kwargs):
        return Arc(center=Vector3d(center.x, center.y), radius=radius,
                   start_angle=start_angle, end_angle=end_angle, **kwargs)


# @dataclass
# class Circle(Curve):
#     pass


# @dataclass
# class Ellipse(Curve):
#     pass


# @dataclass
# class Leader(Curve):
#     pass


@dataclass
class Line(Curve):
    start_point: Optional[Vector3d] = None
    end_point: Optional[Vector3d] = None
    thickness: Optional[float] = None

    @staticmethod
    def new(start_x: Number, start_y: Number,
            end_x: Number, end_y: Number, **kwargs):
        return Line(start_point=Vector3d(start_x, start_y),
                    end_point=Vector3d(end_x, end_y), **kwargs)

    @staticmethod
    def new_xyz(start_x: Number, start_y: Number, start_z: Number,
                end_x: Number, end_y: Number, end_z: Number, **kwargs):
        return Line(start_point=Vector3d(start_x, start_y, start_z),
                    end_point=Vector3d(end_x, end_y, end_z), **kwargs)

    @staticmethod
    def new_vec2(start: Vector2d, end: Vector2d, **kwargs):
        return Line(start_point=Vector3d(start.x, start.y),
                    end_point=Vector3d(end.x, end.y), **kwargs)

    @staticmethod
    def new_vec3(start: Vector3d, end: Vector3d, **kwargs):
        return Line(start_point=start, end_point=end, **kwargs)


@dataclass
class Vertex(Jsonify):
    point: Vector2d
    bulge: Optional[float] = None
    start_width: Optional[float] = None
    end_width: Optional[float] = None

    @staticmethod
    def new(x: Number, y: Number, **kwargs):
        return Vertex(Vector2d(x, y), **kwargs)


@dataclass
class Polyline(Curve):
    closed: Optional[bool] = None
    constant_width: Optional[float] = None
    elevation: Optional[float] = None
    normal: Optional[Vector3d] = None
    thickness: Optional[float] = None
    vertices: Optional[List[Vertex]] = None

    @staticmethod
    def new(*vertices: Vertex, **kwargs):
        return Polyline(vertices=list(vertices), **kwargs)


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


@dataclass
class BlockTableRecord(SymbolTableRecord):
    entities: List[Entity] = field(default_factory=list)


# @dataclass
# class DimStyleTableRecord(SymbolTableRecord, DimStyleCommon):
#     pass


@dataclass
class LayerTableRecord(SymbolTableRecord):
    color: Optional[Color] = None
    is_frozen: Optional[bool] = None
    is_locked: Optional[bool] = None
    is_off: Optional[bool] = None
    is_plottable: Optional[bool] = None
    line_weight: Optional[LineWeight] = None
    linetype: Optional[str] = None


@dataclass
class LinetypeSegment(Jsonify):
    dash_length: Optional[float] = None
    shape_is_ucs_oriented: Optional[bool] = None
    shape_number: Optional[int] = None
    shape_offset: Optional[Vector2d] = None
    shape_rotation: Optional[float] = None
    shape_scale: Optional[float] = None
    shape_style: Optional[str] = None
    text: Optional[str] = None


@dataclass
class LinetypeTableRecord(SymbolTableRecord):
    comments: Optional[str] = None
    segments: Optional[List[LinetypeSegment]] = None

    # Accesses the alignment type for the LinetypeTableRecord. If ScaledToFit is
    # true, the alignment wll be "scaled to fit" (equivalent to an 'S' in the
    # alignment field of the linetype definition). If ScaledToFit is false, the
    # alignment will not be "scaled to fit" (equivalent to an 'A' in the
    # alignment field of the linetype definition).
    is_scaled_to_fit: Optional[bool] = None

    # Accesses the length (in AutoCAD drawing units--the pattern will appear
    # this length when the linetype scale is 1.0) of the pattern in the
    # LinetypeTableRecord. The pattern length is the total length of all dashes
    # (including pen up spaces). Embedded shapes or text strings do not add to
    # the pattern length because they are overlaid and do not interrupt the
    # actual dash pattern. For more information on linetype definitions, see the
    # "Linetypes" section of the AutoCAD Customization Guide.
    pattern_length: Optional[float] = None


@dataclass
class FontDescriptor(Jsonify):
    bold: Optional[bool] = None
    character_set: Optional[int] = None
    italic: Optional[bool] = None
    pitch_and_family: Optional[int] = None
    type_face: Optional[str] = None


@dataclass
class TextStyleTableRecord(SymbolTableRecord):
    font: Optional[FontDescriptor] = None
    big_font_file_name: Optional[str] = None
    file_name: Optional[str] = None
    is_shape_file: Optional[bool] = None
    is_vertical: Optional[bool] = None
    obliquing_angle: Optional[float] = None
    text_size: Optional[float] = None
    x_scale: Optional[float] = None

    # Returns the textStyle flagBits value. Only the second and third bits are
    # used. If the second bit is set it indicates that the text is drawn
    # backward (that is, mirrored in X). If the third bit is set it indicates
    # that the text is drawn upside down (that is, mirrored in Y).
    flag_bits: Optional[int] = None

    # Returns the text height used for the last text created using this Text
    # Style. This value is updated automatically by AutoCAD after the creation
    # of any text object that references this TextStyleTableRecord. If the
    # textSize value for this textStyle is 0, then the priorSize value is used
    # by AutoCAD as the default text height for the next text created using this
    # Text Style.
    prior_size: Optional[float] = None


@dataclass
class Database(Jsonify):
    block_table: Dict[str, BlockTableRecord] = field(default_factory=dict)
    layer_table: Dict[str, LayerTableRecord] = field(default_factory=dict)
    linetype_table: Dict[str, LinetypeTableRecord] = field(default_factory=dict)
    text_style_table: Dict[str, TextStyleTableRecord] = field(
        default_factory=dict)

    def get_block(self, name):
        if name not in self.block_table:
            self.block_table[name] = BlockTableRecord()
        return self.block_table[name]


# Syntactic sugar of @decorator may somehow break the code completion of IDE
# (e.g. PyCharm) on @dataclass.

Arc = csharp_polymorphic_type("SacadMgd.Arc, SacadMgd")(Arc)
Line = csharp_polymorphic_type("SacadMgd.Line, SacadMgd")(Line)
Polyline = csharp_polymorphic_type("SacadMgd.Polyline, SacadMgd")(Polyline)

BlockTableRecord = csharp_polymorphic_type(
    "SacadMgd.BlockTableRecord, SacadMgd")(BlockTableRecord)
LinetypeSegment = csharp_polymorphic_type(
    "SacadMgd.LinetypeSegment, SacadMgd")(LinetypeSegment)
LinetypeTableRecord = csharp_polymorphic_type(
    "SacadMgd.LinetypeTableRecord, SacadMgd")(LinetypeTableRecord)
LayerTableRecord = csharp_polymorphic_type(
    "SacadMgd.LayerTableRecord, SacadMgd")(LayerTableRecord)
TextStyleTableRecord = csharp_polymorphic_type(
    "SacadMgd.TextStyleTableRecord, SacadMgd")(TextStyleTableRecord)
