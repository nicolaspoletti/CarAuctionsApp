import { Button, ButtonGroup } from 'flowbite-react';
import { usePathname } from 'next/navigation';
import React from 'react'
import { useParamsStore } from '../hooks/useParamsStore';
import { AiOutlineClockCircle, AiOutlineSortAscending } from 'react-icons/ai';
import { BsFillStopCircleFill, BsStopwatchFill } from 'react-icons/bs';
import { IconBase } from 'react-icons';
import { GiFinishLine, GiFlame } from 'react-icons/gi';


const pageSizeButtons = [4, 8, 12];

const orderButtons = [
    {
        label: 'Alphabetical',
        icon: AiOutlineSortAscending,
        value: 'make'
    },
    {
        label: 'End date',
        icon: AiOutlineClockCircle,
        value: 'endingSoon'
    },
    {
        label: 'Recently Added',
        icon: BsFillStopCircleFill,
        value: 'new'
    }
];

const filterButtons = [
    {
        label: 'Live Auctions',
        icon: GiFlame,
        value: 'live'
    },
    {
        label: 'Ending soon',
        icon: GiFinishLine,
        value: 'endingSoon'
    },
    {
        label: 'Completed',
        icon: BsStopwatchFill,
        value: 'finished'
    }
];

export default function Filters() {

    const pageSize = useParamsStore(state => state.pageSize);
    const setParams = useParamsStore(state => state.setParams);
    const orderBy = useParamsStore(state => state.orderBy)
    const filterBy = useParamsStore(state => state.filterBy)

    return (
    <div className='flex justify-between items-center mb-4'>

        <div>
            <span className='uppercase text-sm text-gray-500 mr-2'>Filter by</span>
            <Button.Group>
                {filterButtons.map(({label, icon: IconBase, value}) => (
                    <Button
                        key={value}
                        onClick={() => setParams({filterBy: value})}
                        color={`${filterBy === value ? 'red' : 'grey'}`}
                        className='focus:ring-0'
                    >
                        <IconBase className='mr-3 h-4 w'/>
                        {label}
                    </Button>
                ))}
            </Button.Group>
        </div>

        <div>
            <span className='uppercase text-sm text-gray-500 mr-2'>Order by</span>
            <Button.Group>
                {orderButtons.map(({label, icon: IconBase, value}) => (
                    <Button
                        key={value}
                        onClick={() => setParams({orderBy: value})}
                        color={`${orderBy === value ? 'red' : 'grey'}`}
                        className='focus:ring-0'
                    >
                        <IconBase className='mr-3 h-4 w'/>
                        {label}
                    </Button>
                ))}
            </Button.Group>
        </div>

        <div>
            <span className='uppercase text-sm text-gray-500 mr-2'>Page size</span>
            <Button.Group>
                {pageSizeButtons.map((value, i) => (
                    <Button key = {i} 
                        onClick={() => setParams({pageSize: value})}
                        color={`${pageSize === value ? 'red' : 'gray'}`}
                        className='focus:ring-0'
                    >
                        <IconBase >

                        </IconBase>
                        {value}
                    </Button>
                ))}
            </Button.Group>
        </div>
    </div>
  )
}
