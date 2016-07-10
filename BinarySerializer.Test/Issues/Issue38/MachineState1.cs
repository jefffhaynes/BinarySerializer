using System;
using System.Collections.ObjectModel;

namespace BinarySerialization.Test.Issues.Issue38
{
    /// <summary>
    ///     machine state message
    /// </summary>
    public class MachineState1
    {
        #region constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="MachineState1" /> class.
        /// </summary>
        public MachineState1()
            : this(0, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MachineState1" /> class.
        /// </summary>
        /// <param name="posDataCnt">
        ///     initial value for count of position data
        /// </param>
        /// <param name="posData">
        ///     initial list of position data
        /// </param>
        /// <remarks>
        ///     this constructor is used by the serializer. Because of there is no setter for the properties PosDataCnt and PosData
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        public MachineState1(ushort posDataCnt, Collection<PositionData1> posData)
        {
            if (posDataCnt != (posData?.Count ?? 0))
            {
                throw new ArgumentException("Position data count does not match with list of postion data",
                    nameof(posDataCnt));
            }

            PosData = posData ?? new Collection<PositionData1>();
        }

        #endregion

        #region public properties

        /// <summary>
        ///     Gets or sets the position data count
        /// </summary>
        [FieldOrder(5)]
        [SerializeAs(SerializedType = SerializedType.UInt2)]
        public ushort PosDataCnt
        {
            get
            {
                return PosData == null
                    ? (byte) 0
                    : PosData.Count > ushort.MaxValue ? ushort.MaxValue : Convert.ToUInt16(PosData.Count);
            }
        }

        /// <summary>
        ///     Gets or sets the data of all postions.
        /// </summary>
        [FieldOrder(6)]
        [FieldCount("PosDataCnt")]
        public Collection<PositionData1> PosData { get; }

        #endregion
    }
}