import React, { useEffect, useState } from 'react'
import { Input } from './ui/input';
import { Button } from './ui/button';
import { X } from 'lucide-react';
import clsx from 'clsx';

interface Props {
  inputType: string;
  value: any;
  setValue: React.Dispatch<React.SetStateAction<any>>;
  className?: string;
  min?: number;
}

export default function ClearableInput(props: Props) {
  const [value, setValue] = useState<any>("");
  useEffect(() => {
    setValue(value);
  }, [props.value]);

  useEffect(() => {
    props.setValue(value);
  }, [value]);


  return (
    <div className={clsx('relative flex flex-row gap-1 items-center', props.className)}>
      <Input type={props.inputType} 
        value={value}
        onChange={(e) => setValue(e.target.value)}
        min={props.min}  
        className="[-moz-appearance:_textfield] [&::-webkit-inner-spin-button]:m-0 [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:m-0 [&::-webkit-outer-spin-button]:appearance-none"
      />
      {value && (
        <Button
          size="icon"
          variant="ghost"
          className="absolute right-0 top-1/2 hover:bg-transparent hover:text-foreground  -translate-y-1/2"
          onClick={() => setValue("")}
        >
          <X />
        </Button>
      )}
    </div>
    
  )
}
