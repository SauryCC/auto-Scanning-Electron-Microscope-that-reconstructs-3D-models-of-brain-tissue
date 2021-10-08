using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace MIcrotome_GUI
{
    // [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
    class CSmarAct
    {
        //[DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint SA_GetDLLVersion(ref uint version);        

        //[DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint SA_GetNumberOfChannels(uint systemIndex, ref uint channels);

        /**********************
        General note:
        All functions have a return value of SA_STATUS
        indicating success (SA_OK) or failure of execution. See the above
        definitions for a list of error codes.
        ***********************/
        
        /************************************************************************
        *************************************************************************
        **                 Section I: Initialization Functions                 **
        *************************************************************************   
        ************************************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_AddSystemToInitSystemsList(uint systemId);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_ClearInitSystemsList();

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetAvailableSystems(ref uint idList, ref uint idListSize);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetChannelType(uint systemIndex, uint channelIndex, ref uint type);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetDLLVersion(ref uint version);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetInitState(ref uint initMode);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetNumberOfChannels(uint systemIndex, ref uint channels);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetNumberOfSystems(ref uint number);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetSystemID(uint systemIndex, ref uint systemId);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_InitSystems(uint configuration);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_ReleaseSystems();

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_SetHCMEnabled(uint systemIndex, uint enabled);


        /// <summary>
        /// not in use 
        /// </summary>
        /// <param name="systemIndex"></param>
        /// <param name="locator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern uint SA_OpenSystem(ref uint systemIndex, [MarshalAs(UnmanagedType.LPStr)] string locator, [MarshalAs(UnmanagedType.LPStr)] string options);
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern uint SA_CloseSystem(uint systemIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern uint SA_FindSystems([MarshalAs(UnmanagedType.LPStr)] string options,   [MarshalAs(UnmanagedType.LPStr)]  ref string outBuffer, ref uint ioBufferSize);
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern uint SA_GetSystemLocator(uint systemIndex,   [MarshalAs(UnmanagedType.LPStr)] ref string outBuffer, ref uint ioBufferSize);

        ////////////////////////////////



        /************************************************************************
        *************************************************************************
        **        Section IIa:  Functions for SYNCHRONOUS communication        **
        *************************************************************************
        ************************************************************************/

        /*************************************************
        **************************************************
        **    Section IIa.1: Configuration Functions    **
        **************************************************
        *************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetAngleLimit_S(uint systemIndex, uint channelIndex, ref uint minAngle, ref int minRevolution, ref uint maxAngle, ref int maxRevolution);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetClosedLoopMoveSpeed_S(uint systemIndex, uint channelIndex, ref uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GetEndEffectorType_S(uint systemIndex, uint channelIndex, ref uint type, ref int param1, ref int param2);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetPositionLimit_S(uint systemIndex, uint channelIndex, ref int minPosition, ref int maxPosition);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetSafeDirection_S(uint systemIndex, uint channelIndex, ref uint direction);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetScale_S(uint systemIndex, uint channelIndex, ref int scale, ref uint reserved);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Global
        public static extern uint SA_GetSensorEnabled_S(uint systemIndex, ref uint enabled);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetSensorType_S(uint systemIndex, uint channelIndex, ref uint type);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetAccumulateRelativePositions_S(uint systemIndex, uint channelIndex, uint accumulate);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetAngleLimit_S(uint systemIndex, uint channelIndex, uint minAngle, int minRevolution, uint maxAngle, int maxRevolution);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetClosedLoopMaxFrequency_S(uint systemIndex, uint channelIndex, uint frequency);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetClosedLoopMoveSpeed_S(uint systemIndex, uint channelIndex, uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_SetEndEffectorType_S(uint systemIndex, uint channelIndex, uint type, int param1, int param2);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetPosition_S(uint systemIndex, uint channelIndex, int position);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetPositionLimit_S(uint systemIndex, uint channelIndex, int minPosition, int maxPosition);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetSafeDirection_S(uint systemIndex, uint channelIndex, uint direction);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetScale_S(uint systemIndex, uint channelIndex, int scale, uint reserved);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Global
        public static extern uint SA_SetSensorEnabled_S(uint systemIndex, uint enabled);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetSensorType_S(uint systemIndex, uint channelIndex, uint type);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetStepWhileScan_S(uint systemIndex, uint channelIndex, uint step);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_SetZeroForce_S(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetZeroPosition_S(uint systemIndex, uint channelIndex);

        /*************************************************
        **************************************************
        **  Section IIa.2: Movement Control Functions   **
        **************************************************
        *************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_CalibrateSensor_S(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_FindReferenceMark_S(uint systemIndex, uint channelIndex, uint direction, uint holdTime, uint autoZero);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoAngleAbsolute_S(uint systemIndex, uint channelIndex, uint angle, int revolution, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoAngleRelative_S(uint systemIndex, uint channelIndex, int angleDiff, int revolutionDiff, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GotoGripperForceAbsolute_S(uint systemIndex, uint channelIndex, int force, uint speed, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GotoGripperOpeningAbsolute_S(uint systemIndex, uint channelIndex, uint opening, uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GotoGripperOpeningRelative_S(uint systemIndex, uint channelIndex, int diff, uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoPositionAbsolute_S(uint systemIndex, uint channelIndex, int position, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoPositionRelative_S(uint systemIndex, uint channelIndex, int diff, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_ScanMoveAbsolute_S(uint systemIndex, uint channelIndex, uint target, uint scanSpeed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_ScanMoveRelative_S(uint systemIndex, uint channelIndex, int diff, uint scanSpeed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_StepMove_S(uint systemIndex, uint channelIndex, int steps, uint amplitude, uint frequency);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner, End effector
        public static extern uint SA_Stop_S(uint systemIndex, uint channelIndex);

        /************************************************
        *************************************************
        **  Section IIa.3: Channel Feedback Functions  **
        *************************************************
        *************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetAngle_S(uint systemIndex, uint channelIndex, ref uint angle, ref int revolution);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GetForce_S(uint systemIndex, uint channelIndex, ref int force);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GetGripperOpening_S(uint systemIndex, uint channelIndex, ref uint opening);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetPhysicalPositionKnown_S(uint systemIndex, uint channelIndex, ref uint known);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetPosition_S(uint systemIndex, uint channelIndex, ref int position);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner, End effector
        public static extern uint SA_GetStatus_S(uint systemIndex, uint channelIndex, ref uint status);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetVoltageLevel_S(uint systemIndex, uint channelIndex, ref uint level);

        /************************************************************************
        *************************************************************************
        **       Section IIb:  Functions for ASYNCHRONOUS communication        **
        *************************************************************************
        ************************************************************************/

        /*************************************************
        **************************************************
        **    Section IIb.1: Configuration Functions    **
        **************************************************
        *************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_FlushOutput_A(uint systemIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetAngleLimit_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetBufferedOutput_A(uint systemIndex, ref uint mode);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetClosedLoopMoveSpeed_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GetEndEffectorType_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetPhysicalPositionKnown_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetPositionLimit_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetSafeDirection_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetScale_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_GetSensorEnabled_A(uint systemIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetSensorType_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetAccumulateRelativePositions_A(uint systemIndex, uint channelIndex, uint accumulate);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetAngleLimit_A(uint systemIndex, uint channelIndex, uint minAngle, int minRevolution, uint maxAngle, int maxRevolution);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_SetBufferedOutput_A(uint systemIndex, uint mode);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetClosedLoopMaxFrequency_A(uint systemIndex, uint channelIndex, uint frequency);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetClosedLoopMoveSpeed_A(uint systemIndex, uint channelIndex, uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_SetEndEffectorType_A(uint systemIndex, uint channelIndex, uint type, int param1, int param2);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetPosition_A(uint systemIndex, uint channelIndex, int position);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetPositionLimit_A(uint systemIndex, uint channelIndex, int minPosition, int maxPosition);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner, End effector
        public static extern uint SA_SetReportOnComplete_A(uint systemIndex, uint channelIndex, uint report);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetSafeDirection_A(uint systemIndex, uint channelIndex, uint direction);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetScale_A(uint systemIndex, uint channelIndex, int scale, uint reserved);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_SetSensorEnabled_A(uint systemIndex, uint enabled);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetSensorType_A(uint systemIndex, uint channelIndex, uint type);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetStepWhileScan_A(uint systemIndex, uint channelIndex, uint step);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_SetZeroForce_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_SetZeroPosition_A(uint systemIndex, uint channelIndex);

        /*************************************************
        **************************************************
        **  Section IIb.2: Movement Control Functions   **
        **************************************************
        *************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_CalibrateSensor_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_FindReferenceMark_A(uint systemIndex, uint channelIndex, uint direction, uint holdTime, uint autoZero);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoAngleAbsolute_A(uint systemIndex, uint channelIndex, uint angle, int revolution, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoAngleRelative_A(uint systemIndex, uint channelIndex, int angleDiff, int revolutionDiff, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GotoGripperForceAbsolute_A(uint systemIndex, uint channelIndex, int force, uint speed, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GotoGripperOpeningAbsolute_A(uint systemIndex, uint channelIndex, uint opening, uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GotoGripperOpeningRelative_A(uint systemIndex, uint channelIndex, int diff, uint speed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoPositionAbsolute_A(uint systemIndex, uint channelIndex, int position, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GotoPositionRelative_A(uint systemIndex, uint channelIndex, int diff, uint holdTime);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_ScanMoveAbsolute_A(uint systemIndex, uint channelIndex, uint target, uint scanSpeed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_ScanMoveRelative_A(uint systemIndex, uint channelIndex, int diff, uint scanSpeed);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_StepMove_A(uint systemIndex, uint channelIndex, int steps, uint amplitude, uint frequency);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner, End effector
        public static extern uint SA_Stop_A(uint systemIndex, uint channelIndex);

        /************************************************
        *************************************************
        **  Section IIb.3: Channel Feedback Functions  **
        *************************************************
        ************************************************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetAngle_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GetForce_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // End effector
        public static extern uint SA_GetGripperOpening_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetPosition_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner, End effector
        public static extern uint SA_GetStatus_A(uint systemIndex, uint channelIndex);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)] // Positioner
        public static extern uint SA_GetVoltageLevel_A(uint systemIndex, uint channelIndex);

        /******************
        * Answer retrieval
        ******************/
        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_DiscardPacket_A(uint systemIndex);

        //[DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint SA_LookAtNextPacket_A(uint  systemIndex, uint timeout, SA_PACKET *packet);

        //[DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint SA_ReceiveNextPacket_A(uint  systemIndex, uint timeout, SA_PACKET *packet);

        //[DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        //public static extern uint SA_ReceiveNextPacketIfChannel_A(uint  systemIndex, uint  channelIndex, uint timeout, SA_PACKET *packet);

        [DllImport(@"MCSControl.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint SA_SetReceiveNotification_A(uint systemIndex, ref uint var_event);

        // function status return values;
        public const uint SA_OK = 0;
        public const uint SA_INITIALIZATION_ERROR = 1;
        public const uint SA_NOT_INITIALIZED_ERROR = 2;
        public const uint SA_NO_SYSTEMS_FOUND_ERROR = 3;
        public const uint SA_TOO_MANY_SYSTEMS_ERROR = 4;
        public const uint SA_INVALID_SYSTEM_INDEX_ERROR = 5;
        public const uint SA_INVALID_CHANNEL_INDEX_ERROR = 6;
        public const uint SA_TRANSMIT_ERROR = 7;
        public const uint SA_WRITE_ERROR = 8;
        public const uint SA_INVALID_PARAMETER_ERROR = 9;
        public const uint SA_READ_ERROR = 10;
        public const uint SA_INTERNAL_ERROR = 12;
        public const uint SA_WRONG_MODE_ERROR = 13;
        public const uint SA_PROTOCOL_ERROR = 14;
        public const uint SA_TIMEOUT_ERROR = 15;
        public const uint SA_NOTIFICATION_ALREADY_SET_ERROR = 16;
        public const uint SA_ID_LIST_TOO_SMALL_ERROR = 17;
        public const uint SA_SYSTEM_ALREADY_ADDED_ERROR = 18;
        public const uint SA_WRONG_CHANNEL_TYPE_ERROR = 19;
        public const uint SA_NO_SENSOR_PRESENT_ERROR = 129;
        public const uint SA_AMPLITUDE_TOO_LOW_ERROR = 130;
        public const uint SA_AMPLITUDE_TOO_HIGH_ERROR = 131;
        public const uint SA_FREQUENCY_TOO_LOW_ERROR = 132;
        public const uint SA_FREQUENCY_TOO_HIGH_ERROR = 133;
        public const uint SA_SCAN_TARGET_TOO_HIGH_ERROR = 135;
        public const uint SA_SCAN_SPEED_TOO_LOW_ERROR = 136;
        public const uint SA_SCAN_SPEED_TOO_HIGH_ERROR = 137;
        public const uint SA_SENSOR_DISABLED_ERROR = 140;
        public const uint SA_COMMAND_OVERRIDDEN_ERROR = 141;
        public const uint SA_END_STOP_REACHED_ERROR = 142;
        public const uint SA_WRONG_SENSOR_TYPE_ERROR = 143;
        public const uint SA_COULD_NOT_FIND_REF_ERROR = 144;
        public const uint SA_WRONG_END_EFFECTOR_TYPE_ERROR = 145;
        public const uint SA_RANGE_LIMIT_REACHED_ERROR = 147;
        public const uint SA_PHYSICAL_POSITION_UNKNOWN_ERROR = 148;
        public const uint SA_OUTPUT_BUFFER_OVERFLOW_ERROR = 149;
        public const uint SA_UNKNOWN_COMMAND_ERROR = 240;
        public const uint SA_OTHER_ERROR = 255;

        // configuration flags for SA_InitDevices;
        public const uint SA_SYNCHRONOUS_COMMUNICATION = 0;
        public const uint SA_ASYNCHRONOUS_COMMUNICATION = 1;
        public const uint SA_HARDWARE_RESET = 2;

        // return values from SA_GetInitState;
        public const uint SA_INIT_STATE_NONE = 0;
        public const uint SA_INIT_STATE_SYNC = 1;
        public const uint SA_INIT_STATE_ASYNC = 2;

        // configuration flags for SA_SetStepWhileScan_X;
        public const uint SA_NO_STEP_WHILE_SCAN = 0;
        public const uint SA_STEP_WHILE_SCAN = 1;

        // configuration flags for SA_SetSensorEnabled_X;
        public const uint SA_SENSOR_DISABLED = 0;
        public const uint SA_SENSOR_ENABLED = 1;
        public const uint SA_SENSOR_POWERSAVE = 2;

        // configuration flags for SA_SetReportOnComplete_A;
        public const uint SA_NO_REPORT_ON_COMPLETE = 0;
        public const uint SA_REPORT_ON_COMPLETE = 1;

        // configuration flags for SA_SetAccumulateRelativePositions_X;
        public const uint SA_NO_ACCUMULATE_RELATIVE_POSITIONS = 0;
        public const uint SA_ACCUMULATE_RELATIVE_POSITIONS = 1;

        // packet types (for asynchronous mode);
        public const uint SA_NO_PACKET_TYPE = 0;
        public const uint SA_ERROR_PACKET_TYPE = 1;
        public const uint SA_POSITION_PACKET_TYPE = 2;
        public const uint SA_COMPLETED_PACKET_TYPE = 3;
        public const uint SA_STATUS_PACKET_TYPE = 4;
        public const uint SA_ANGLE_PACKET_TYPE = 5;
        public const uint SA_VOLTAGE_LEVEL_PACKET_TYPE = 6;
        public const uint SA_SENSOR_TYPE_PACKET_TYPE = 7;
        public const uint SA_SENSOR_ENABLED_PACKET_TYPE = 8;
        public const uint SA_END_EFFECTOR_TYPE_PACKET_TYPE = 9;
        public const uint SA_GRIPPER_OPENING_PACKET_TYPE = 10;
        public const uint SA_FORCE_PACKET_TYPE = 11;
        public const uint SA_MOVE_SPEED_PACKET_TYPE = 12;
        public const uint SA_PHYSICAL_POSITION_KNOWN_PACKET_TYPE = 13;
        public const uint SA_POSITION_LIMIT_PACKET_TYPE = 14;
        public const uint SA_ANGLE_LIMIT_PACKET_TYPE = 15;
        public const uint SA_SAFE_DIRECTION_PACKET_TYPE = 16;
        public const uint SA_SCALE_PACKET_TYPE = 17;
        public const uint SA_INVALID_PACKET_TYPE = 255;

        // channel status codes;
        public const uint SA_STOPPED_STATUS = 0;
        public const uint SA_STEPPING_STATUS = 1;
        public const uint SA_SCANNING_STATUS = 2;
        public const uint SA_HOLDING_STATUS = 3;
        public const uint SA_TARGET_STATUS = 4;
        public const uint SA_MOVE_DELAY_STATUS = 5;
        public const uint SA_CALIBRATING_STATUS = 6;
        public const uint SA_FINDING_REF_STATUS = 7;
        public const uint SA_OPENING_STATUS = 8;

        // HCM enabled levels (for SA_SetHCMEnabled);
        public const uint SA_HCM_DISABLED = 0;
        public const uint SA_HCM_ENABLED = 1;
        public const uint SA_HCM_CONTROLS_DISABLED = 2;

        // sensor types (for SA_SetSensorType_X and SA_GetSensorType_X);
        public const uint SA_NO_SENSOR_TYPE = 0;
        public const uint SA_S_SENSOR_TYPE = 1;
        public const uint SA_SR_SENSOR_TYPE = 2;
        public const uint SA_ML_SENSOR_TYPE = 3;
        public const uint SA_MR_SENSOR_TYPE = 4;
        public const uint SA_SP_SENSOR_TYPE = 5;
        public const uint SA_SC_SENSOR_TYPE = 6;
        public const uint SA_M25_SENSOR_TYPE = 7;
        public const uint SA_SR20_SENSOR_TYPE = 8;
        public const uint SA_M_SENSOR_TYPE = 9;
        public const uint SA_GC_SENSOR_TYPE = 10;
        public const uint SA_GD_SENSOR_TYPE = 11;
        public const uint SA_GE_SENSOR_TYPE = 12;
        public const uint SA_RA_SENSOR_TYPE = 13;
        public const uint SA_GF_SENSOR_TYPE = 14;
        public const uint SA_RB_SENSOR_TYPE = 15;
        public const uint SA_G605S_SENSOR_TYPE = 16;
        public const uint SA_G775S_SENSOR_TYPE = 17;

        // compatibility definitions;
        public const uint SA_LIN20UMS_SENSOR_TYPE = SA_S_SENSOR_TYPE;
        public const uint SA_ROT3600S_SENSOR_TYPE = SA_SR_SENSOR_TYPE;
        public const uint SA_ROT50LS_SENSOR_TYPE = SA_ML_SENSOR_TYPE;
        public const uint SA_ROT50RS_SENSOR_TYPE = SA_MR_SENSOR_TYPE;
        public const uint SA_LINEAR_SENSOR_TYPE = SA_S_SENSOR_TYPE;
        public const uint SA_ROTARY_SENSOR_TYPE = SA_SR_SENSOR_TYPE;

        // movement directions (for SA_FindReferenceMark_X);
        public const uint SA_FORWARD_DIRECTION = 0;
        public const uint SA_BACKWARD_DIRECTION = 1;
        public const uint SA_FORWARD_BACKWARD_DIRECTION = 2;
        public const uint SA_BACKWARD_FORWARD_DIRECTION = 3;

        // auto zero (for SA_FindReferenceMark_X);
        public const uint SA_NO_AUTO_ZERO = 0;
        public const uint SA_AUTO_ZERO = 1;

        // physical position (for SA_GetPhyscialPositionKnown_X);
        public const uint SA_PHYSICAL_POSITION_UNKNOWN = 0;
        public const uint SA_PHYSICAL_POSITION_KNOWN = 1;

        // channel types (for SA_GetChannelType);
        public const uint SA_POSITIONER_CHANNEL_TYPE = 0;
        public const uint SA_END_EFFECTOR_CHANNEL_TYPE = 1;

        // end effector types;
        public const uint SA_ANALOG_SENSOR_END_EFFECTOR_TYPE = 0;
        public const uint SA_GRIPPER_END_EFFECTOR_TYPE = 1;
        public const uint SA_FORCE_SENSOR_END_EFFECTOR_TYPE = 2;
        public const uint SA_FORCE_GRIPPER_END_EFFECTOR_TYPE = 3;
        public const uint SA_UNBUFFERED_OUTPUT = 0;
        public const uint SA_BUFFERED_OUTPUT = 1;

        // channel number;
        public const uint SA_Z_AXIS = 0;

    }
}
//Now the functions can be used in the C# program:

//    uint version = 0;

//    MCS.SA_GetDLLVersion(ref version);

//    Console.WriteLine("Version: " + version);

//}