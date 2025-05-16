import * as React from "react";
import { Button } from "@/components/ui/button";
import { Check } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import { cn } from "@/lib/utils";

interface HoverButtonProps extends React.ComponentPropsWithoutRef<"button"> {
  children: React.ReactNode;
}

export const HoverButton: React.FC<HoverButtonProps> = ({
  children,
  className,
  ...props
}) => {
  const [hovered, setHovered] = React.useState(false);

  return (
    <Button
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      className={cn(
        "relative overflow-hidden px-6 py-3 transition-all duration-300 ease-in-out bg-blue-500 text-white hover:bg-gradient-to-r hover:from-green-400 hover:to-blue-500",
        className
      )}
      {...props}
    >
      <div className="flex items-center space-x-2">
        <AnimatePresence>
          {hovered && (
            <motion.span
              initial={{ x: -20, opacity: 0 }}
              animate={{ x: 0, opacity: 1 }}
              exit={{ x: -20, opacity: 0 }}
              transition={{ duration: 0.2 }}
              className="flex items-center"
            >
              <Check className="w-4 h-4" />
            </motion.span>
          )}
        </AnimatePresence>
        <span className="whitespace-nowrap">{children}</span>
      </div>
    </Button>
  );
};
