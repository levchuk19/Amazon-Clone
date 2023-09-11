import classNames from 'classnames';
import React, { useState } from 'react'
import { Link, Outlet } from 'react-router-dom'



function SellerWrap() {

   var [selectedTab,setSelectedTab]= useState("Моя компанія");

  return (
    <div className='mx-auto w-10/12'>
        <div className='  bg-gray-500 rounded-xl '>
            <div className='flex h-full '>
                <Link to={'mycompany'} className={classNames(' cursor-pointer duration-200 px-10 flex rounded-t-lg border-t border-x border-gray-500 h-full py-2  font-medium transition-all ',{" bg-white text-black  border-gray-200":selectedTab == "Моя компанія","text-white":selectedTab != "Моя компанія"})} onClick={()=>{setSelectedTab("Моя компанія")}}>Моя компанія</Link>
                <Link to={'toOrders'}  className={classNames(' cursor-pointer duration-200 px-10 flex rounded-t-lg border-t border-x border-gray-500 h-full py-2  font-medium transition-all ',{" bg-white text-black  border-gray-200":selectedTab == "Замовлені товари","text-white":selectedTab != "Замовлені товари"})}  onClick={()=>{setSelectedTab("Замовлені товари")}}>Замовлені товари</Link>
            </div>
            <div className=' border py-5 bg-white rounded-b-lg px-10'>
                <Outlet/>
            </div>
        </div>
    </div>
   
  )
}


export default SellerWrap
