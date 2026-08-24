import type { ReactNode } from "react";
import { ArrowRight, Blocks } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from "./ui/button";

export type DashboardAction = {
  title: string;
  description: string;
  icon: typeof Blocks;
  buttonLabel: string;
  onClick: () => void;
  isPrimary?: boolean;
  children?: ReactNode;
};

const DashboardActionCard = (props: DashboardAction) => {
  return (
    <Card className="border-border/60 bg-card/80 rounded-xl backdrop-blur">
      <CardHeader className="space-y-4">
        <div className="flex h-12 w-12 items-center justify-center rounded-2xl bg-primary/10 text-primary">
          <props.icon className="h-6 w-6" />
        </div>
        <div className="space-y-2">
          <CardTitle className="text-lg">{props.title}</CardTitle>
          <CardDescription className="text-sm leading-6">{props.description}</CardDescription>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        {props.children}
        <Button className="w-full justify-between" variant={props.isPrimary ? "default" : "outline"} onClick={props.onClick}>
          <span>{props.buttonLabel}</span>
          <ArrowRight className="h-4 w-4" />
        </Button>
      </CardContent>
    </Card>
  );
}

export default DashboardActionCard