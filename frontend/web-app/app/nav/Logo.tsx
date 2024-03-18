'use client'

import React from 'react'
import { GiCarWheel } from 'react-icons/gi'
import { useParamsStore } from '../hooks/useParamsStore'

export default function Logo() {

    const reset = useParamsStore(state => state.reset)
    
    return (
    <div onClick={reset} className='cursor-pointer flex items-center gap-2 text-3xl font-semibold text-red-500'>
        <GiCarWheel size={34}/>
        <div>Car Auctions</div>
    </div>
  )
}
