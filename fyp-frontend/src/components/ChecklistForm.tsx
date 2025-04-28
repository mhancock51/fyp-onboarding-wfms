import React from 'react'
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Trash2 } from 'lucide-react';
import { Textarea } from './ui/textarea';

export default function ChecklistForm(props: { items: string[]; setItems: React.Dispatch<React.SetStateAction<string[]>>, restrictInputs: boolean}) {
  function addEmptyItem() {
    props.setItems((prevState) => ([
      ...prevState, ""
    ]));
  }

  function deleteItem(index: number) {
      props.setItems((prevState) => (prevState.filter((_, i) => (i !== index))))
    }
  
    function updateItem(item: string, index: number) {
      var updatedItems = [...props.items];
      updatedItems[index] = item;
      props.setItems(updatedItems);
    }

  return (
    <div className='w-full flex flex-col gap-2'>
      <div className='max-h-150 overflow-y-auto grid grid-col gap-4'>
        {
          props.items.map((item, index) => (
            <div key={index} className='flex flex-row justify-between items-center'>
              <Textarea disabled={props.restrictInputs} required placeholder='Enter description of task...' className="col-span-3" value={item} onChange={(event: any) => {updateItem(event.target.value, index);}}/>
              <Button disabled={props.restrictInputs} className='my-1 mx-2' onClick={() => {deleteItem(index);}} variant={"destructive"}><Trash2/></Button>
            </div>
          ))
        }
      </div>
      <div className="grid grid-row items-center gap-4">
        <Button onClick={addEmptyItem} disabled={props.restrictInputs}>Add Item</Button>
      </div>
    </div>
  )
}
